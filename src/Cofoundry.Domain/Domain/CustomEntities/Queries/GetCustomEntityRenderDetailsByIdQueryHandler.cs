using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to retreive a custom entity by it's database id, projected as a
    /// CustomEntityRenderDetails, which contains all data for rendering a specific 
    /// version of a custom entity out to a page, including template data for all the 
    /// content-editable page regions. This projection is specific to a particular 
    /// version which may not always be the latest (depending on the query), and to a 
    /// specific page. Although often you may only have one custom entity page, it is 
    /// possible to have multiple.
    /// </summary>
    public class GetCustomEntityRenderDetailsByIdQueryHandler
        : IQueryHandler<GetCustomEntityRenderDetailsByIdQuery, CustomEntityRenderDetails>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IQueryExecutor _queryExecutor;

        public GetCustomEntityRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityDataModelMapper customEntityDataModelMapper,
            IEntityVersionPageBlockMapper entityVersionPageBlockMapper,
            IPermissionValidationService permissionValidationService,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
            _permissionValidationService = permissionValidationService;
            _queryExecutor = queryExecutor;
        }

        public async Task<CustomEntityRenderDetails> ExecuteAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryCustomEntityAsync(query, executionContext);
            if (dbResult == null) return null;

            var entity = MapCustomEntity(dbResult, executionContext);

            if (dbResult.CustomEntity.LocaleId.HasValue)
            {
                var getLocaleQuery = new GetActiveLocaleByIdQuery(dbResult.CustomEntity.LocaleId.Value);
                entity.Locale = await _queryExecutor.ExecuteAsync(getLocaleQuery, executionContext);
            }

            var pageRoutesQuery = new GetPageRoutingInfoByCustomEntityIdQuery(dbResult.CustomEntityId);
            var pageRoutes = await _queryExecutor.ExecuteAsync(pageRoutesQuery, executionContext);
            entity.PageUrls = MapPageRoutings(pageRoutes, dbResult);

            var selectedRoute = pageRoutes.FirstOrDefault(r => r.PageRoute.PageId == query.PageId);

            if (selectedRoute != null)
            {
                var pageVersion = selectedRoute.PageRoute.Versions.GetVersionRouting(PublishStatusQuery.PreferPublished);
                if (pageVersion == null)
                {
                    throw new Exception($"Error mapping routes: {nameof(pageVersion)} cannot be null. A page route should always have at least one version.");
                }

                entity.Regions = await GetRegionsAsync(pageVersion.PageTemplateId);
                var dbPageBlocks = await GetPageBlocksAsync(entity.CustomEntityVersionId, selectedRoute.PageRoute.PageId);

                var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);
                await _entityVersionPageBlockMapper.MapRegionsAsync(dbPageBlocks, entity.Regions, allBlockTypes, query.PublishStatus, executionContext);
            }
            else
            {
                entity.Regions = Array.Empty<CustomEntityPageRegionRenderDetails>();
            }

            return entity;
        }

        private Task<List<CustomEntityVersionPageBlock>> GetPageBlocksAsync(int customEntityVersionId, int pageId)
        {
            return _dbContext
                .CustomEntityVersionPageBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => m.CustomEntityVersionId == customEntityVersionId && m.PageId == pageId)
                .ToListAsync();
        }

        private Task<List<CustomEntityPageRegionRenderDetails>> GetRegionsAsync(int pageTemplateId)
        {
            return _dbContext
                .PageTemplateRegions
                .AsNoTracking()
                .Where(s => s.PageTemplateId == pageTemplateId)
                .Select(s => new CustomEntityPageRegionRenderDetails()
                {
                    PageTemplateRegionId = s.PageTemplateRegionId,
                    Name = s.Name
                })
                .ToListAsync();
        }

        private CustomEntityRenderDetails MapCustomEntity(CustomEntityVersion dbResult, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode, executionContext.UserContext);

            var entity = new CustomEntityRenderDetails()
            {
                CreateDate = DbDateTimeMapper.AsUtc(dbResult.CreateDate),
                CustomEntityDefinitionCode = dbResult.CustomEntity.CustomEntityDefinitionCode,
                CustomEntityId = dbResult.CustomEntityId,
                CustomEntityVersionId = dbResult.CustomEntityVersionId,
                Ordering = dbResult.CustomEntity.Ordering,
                Title = dbResult.Title,
                UrlSlug = dbResult.CustomEntity.UrlSlug,
                WorkFlowStatus = (WorkFlowStatus)dbResult.WorkFlowStatusId,
                PublishDate = DbDateTimeMapper.AsUtc(dbResult.CustomEntity.PublishDate)
            };

            entity.PublishStatus = PublishStatusMapper.FromCode(dbResult.CustomEntity.PublishStatusCode);
            entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);
            
            return entity;
        }

        private async Task<CustomEntityVersion> QueryCustomEntityAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            CustomEntityVersion result;

            if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
            {
                if (!query.CustomEntityVersionId.HasValue)
                {
                    throw new Exception("A CustomEntityVersionId must be included in the query to use PublishStatusQuery.SpecificVersion");
                }

                result = await _dbContext
                    .CustomEntityVersions
                    .AsNoTracking()
                    .Include(e => e.CustomEntity)
                    .FilterActive()
                    .FilterByCustomEntityId(query.CustomEntityId)
                    .FilterByCustomEntityVersionId(query.CustomEntityVersionId.Value)
                    .SingleOrDefaultAsync();
            }
            else
            {
                var dbResult = await _dbContext
                    .CustomEntityPublishStatusQueries
                    .AsNoTracking()
                    .Include(e => e.CustomEntityVersion)
                    .ThenInclude(e => e.CustomEntity)
                    .FilterActive()
                    .FilterByCustomEntityId(query.CustomEntityId)
                    .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                    .SingleOrDefaultAsync();

                result = dbResult?.CustomEntityVersion;
            }

            return result;
        }

        private ICollection<string> MapPageRoutings(
            ICollection<PageRoutingInfo> allRoutings,
            CustomEntityVersion dbResult
            )
        {
            if (allRoutings == null) return Array.Empty<string>();

            var urls = new List<string>(allRoutings.Count());

            foreach (var detailsRouting in allRoutings
                .Where(r => r.CustomEntityRouteRule != null))
            {
                var detailsUrl = detailsRouting
                    .CustomEntityRouteRule
                    .MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);

                urls.Add(detailsUrl);
            }

            return urls;
        }
    }
}
