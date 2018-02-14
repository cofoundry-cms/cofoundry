using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityDetailsByIdQuery, CustomEntityDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
        private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IAuditDataMapper _auditDataMapper;

        public GetCustomEntityDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper,
            IEntityVersionPageBlockMapper entityVersionPageBlockMapper,
            IPermissionValidationService permissionValidationService,
            IAuditDataMapper auditDataMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
            _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
            _permissionValidationService = permissionValidationService;
            _auditDataMapper = auditDataMapper;
        }

        #endregion

        #region execution

        public async Task<CustomEntityDetails> ExecuteAsync(GetCustomEntityDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var customEntityVersion = await Query(query.CustomEntityId).FirstOrDefaultAsync();
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(customEntityVersion.CustomEntity.CustomEntityDefinitionCode);
            
            return await MapAsync(query, customEntityVersion, executionContext);
        }

        private async Task<CustomEntityDetails> MapAsync(
            GetCustomEntityDetailsByIdQuery query, 
            CustomEntityVersion dbVersion,
            IExecutionContext executionContext
            )
        {
            if (dbVersion == null) return null;

            var entity = MapInitialData(dbVersion, executionContext);

            // Re-map IsPublished checking to see if there is a published version in the history
            if (entity.IsPublished && entity.LatestVersion.WorkFlowStatus != WorkFlowStatus.Published)
            {
                entity.IsPublished = await _dbContext
                    .CustomEntityVersions
                    .AnyAsync(v => v.CustomEntityId == query.CustomEntityId && v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            }

            if (dbVersion.CustomEntity.LocaleId.HasValue)
            {
                var getLocaleQuery = new GetActiveLocaleByIdQuery(dbVersion.CustomEntity.LocaleId.Value);
                entity.Locale = await _queryExecutor.ExecuteAsync(getLocaleQuery, executionContext);
            }

            // Custom Mapping
            await MapDataModelAsync(query, dbVersion, entity.LatestVersion, executionContext);

            await MapPages(dbVersion, entity, executionContext);

            return entity;
        }

        private CustomEntityDetails MapInitialData(CustomEntityVersion dbVersion, IExecutionContext executionContext)
        {
            var entity = new CustomEntityDetails()
            {
                CustomEntityId = dbVersion.CustomEntity.CustomEntityId,
                UrlSlug = dbVersion.CustomEntity.UrlSlug,             
                PublishStatus = PublishStatusMapper.FromCode(dbVersion.CustomEntity.PublishStatusCode),
                PublishDate = dbVersion.CustomEntity.PublishDate,
            };

            entity.IsPublished = entity.PublishStatus == PublishStatus.Published && entity.PublishDate <= executionContext.ExecutionDate;
            entity.AuditData = _auditDataMapper.MapCreateAuditData(dbVersion.CustomEntity);

            entity.LatestVersion = new CustomEntityVersionDetails()
            {
                CustomEntityVersionId = dbVersion.CustomEntityVersionId,
                Title = dbVersion.Title,
                WorkFlowStatus = (WorkFlowStatus)dbVersion.WorkFlowStatusId
            };

            entity.LatestVersion.AuditData = _auditDataMapper.MapCreateAuditData(dbVersion);
            entity.HasDraft = entity.LatestVersion.WorkFlowStatus == WorkFlowStatus.Draft;

            return entity;
        }

        private async Task MapPages(CustomEntityVersion dbVersion, CustomEntityDetails entity, IExecutionContext executionContext)
        {
            var pages = new List<CustomEntityPage>();
            entity.LatestVersion.Pages = pages;

            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdQuery(dbVersion.CustomEntityId);
            var routings = (await _queryExecutor.ExecuteAsync(routingsQuery, executionContext))
                .Where(r => r.CustomEntityRouteRule != null);

            if (!routings.Any()) return;
            
            // Map templates

            var pageTemplateIds = routings
                .Select(r => new
                {
                    PageId = r.PageRoute.PageId,
                    VersionRoute = r.PageRoute.Versions.GetVersionRouting(PublishStatusQuery.Latest)
                })
                .Where(r => r.VersionRoute != null && r.VersionRoute.HasCustomEntityRegions)
                .ToDictionary(k => k.PageId, v => v.VersionRoute.PageTemplateId);

            var allTemplateIds = pageTemplateIds
                .Select(r => r.Value)
                .ToArray();

            var allTemplateRegions = await _dbContext
                .PageTemplateRegions
                .AsNoTracking()
                .Where(s => allTemplateIds.Contains(s.PageTemplateId) && s.IsCustomEntityRegion)
                .ToListAsync();

            var allPageBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

            foreach (var routing in routings)
            {
                var page = new CustomEntityPage();
                pages.Add(page);
                page.FullPath = routing.CustomEntityRouteRule.MakeUrl(routing.PageRoute, routing.CustomEntityRoute);
                page.PageRoute = routing.PageRoute;

                // Map Regions

                var templateId = pageTemplateIds.GetOrDefault(routing.PageRoute.PageId);
                page.Regions = allTemplateRegions
                    .Where(s => s.PageTemplateId == templateId)
                    .OrderBy(s => s.UpdateDate)
                    .Select(s => new CustomEntityPageRegionDetails()
                    {
                        Name = s.Name,
                        PageTemplateRegionId = s.PageTemplateRegionId
                    })
                    .ToList();

                // Map Blocks

                foreach (var region in page.Regions)
                {
                    region.Blocks = dbVersion
                        .CustomEntityVersionPageBlocks
                        .Where(m => m.PageTemplateRegionId == region.PageTemplateRegionId)
                        .OrderBy(m => m.Ordering)
                        .Select(m => MapBlock(m, allPageBlockTypes))
                        .ToArray();
                }
            }

            // Map default full path

            entity.FullPath = pages
                .OrderByDescending(p => p.PageRoute.Locale == null)
                .Select(p => p.FullPath)
                .First();
        }

        private CustomEntityVersionPageBlockDetails MapBlock(CustomEntityVersionPageBlock dbBlock, IEnumerable<PageBlockTypeSummary> allPageBlockTypes)
        {
            var blockType = allPageBlockTypes.SingleOrDefault(t => t.PageBlockTypeId == dbBlock.PageBlockTypeId);

            var block = new CustomEntityVersionPageBlockDetails();
            block.BlockType = blockType;
            block.DataModel = _pageVersionBlockModelMapper.MapDataModel(blockType.FileName, dbBlock);
            block.CustomEntityVersionPageBlockId = dbBlock.CustomEntityVersionPageBlockId;
            block.Template = _entityVersionPageBlockMapper.GetCustomTemplate(dbBlock, blockType);

            return block;
        }

        private IQueryable<CustomEntityVersion> Query(int id)
        {
            return _dbContext
                .CustomEntityVersions
                .Include(v => v.CustomEntityVersionPageBlocks)
                .Include(v => v.CustomEntity)
                .ThenInclude(e => e.Creator)
                .Include(v => v.Creator)
                .AsNoTracking()
                .Where(v => v.CustomEntityId == id && (v.CustomEntity.LocaleId == null || v.CustomEntity.Locale.IsActive))
                .OrderByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(g => g.CreateDate);
        }

        private async Task MapDataModelAsync(
            GetCustomEntityDetailsByIdQuery query, 
            CustomEntityVersion dbVersion, 
            CustomEntityVersionDetails version,
            IExecutionContext executionContext
            )
        {
            var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(dbVersion.CustomEntity.CustomEntityDefinitionCode);
            var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
            EntityNotFoundException.ThrowIfNull(definition, dbVersion.CustomEntity.CustomEntityDefinitionCode);

            version.Model = (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(dbVersion.SerializedData, definition.DataModelType);
        }

        #endregion
    }
}
