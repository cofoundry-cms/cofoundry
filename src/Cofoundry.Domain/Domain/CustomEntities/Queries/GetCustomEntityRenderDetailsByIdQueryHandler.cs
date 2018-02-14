using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderDetailsByIdQueryHandler
        : IAsyncQueryHandler<GetCustomEntityRenderDetailsByIdQuery, CustomEntityRenderDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

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

        #endregion

        #region execution

        public async Task<CustomEntityRenderDetails> ExecuteAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryCustomEntityAsync(query, executionContext);
            if (dbResult == null) return null;

            var entity = await MapCustomEntityAsync(dbResult);

            if (dbResult.CustomEntity.LocaleId.HasValue)
            {
                var getLocaleQuery = new GetActiveLocaleByIdQuery(dbResult.CustomEntity.LocaleId.Value);
                entity.Locale = await _queryExecutor.ExecuteAsync(getLocaleQuery, executionContext);
            }

            entity.Regions = await QueryRegions(query).ToListAsync();
            var dbPageBlocks = await QueryPageBlocks(entity).ToListAsync();

            var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);
            await _entityVersionPageBlockMapper.MapRegionsAsync(dbPageBlocks, entity.Regions, allBlockTypes, query.PublishStatus);

            var routingQuery = new GetPageRoutingInfoByCustomEntityIdQuery(dbResult.CustomEntityId);
            var routing = await _queryExecutor.ExecuteAsync(routingQuery, executionContext);
            entity.PageUrls = MapPageRoutings(routing, dbResult);

            return entity;
        }

        private IQueryable<CustomEntityVersionPageBlock> QueryPageBlocks(CustomEntityRenderDetails entity)
        {
            return _dbContext
                .CustomEntityVersionPageBlocks
                .AsNoTracking()
                .Where(m => m.CustomEntityVersionId == entity.CustomEntityVersionId);
        }

        private IQueryable<CustomEntityPageRegionRenderDetails> QueryRegions(GetCustomEntityRenderDetailsByIdQuery query)
        {
            return _dbContext
                .PageTemplateRegions
                .AsNoTracking()
                .Where(s => s.PageTemplateId == query.PageTemplateId)
                .Select(s => new CustomEntityPageRegionRenderDetails()
                {
                    PageTemplateRegionId = s.PageTemplateRegionId,
                    Name = s.Name
                });
        }

        private async Task<CustomEntityRenderDetails> MapCustomEntityAsync(CustomEntityVersion dbResult)
        {
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode);

            var entity = new CustomEntityRenderDetails()
            {
                CreateDate = dbResult.CreateDate,
                CustomEntityDefinitionCode = dbResult.CustomEntity.CustomEntityDefinitionCode,
                CustomEntityId = dbResult.CustomEntityId,
                CustomEntityVersionId = dbResult.CustomEntityVersionId,
                Ordering = dbResult.CustomEntity.Ordering,
                Title = dbResult.Title,
                UrlSlug = dbResult.CustomEntity.UrlSlug,
                WorkFlowStatus = (WorkFlowStatus)dbResult.WorkFlowStatusId,
                PublishDate = dbResult.CustomEntity.PublishDate
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
                    .FilterByActive()
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
                    .FilterByActive()
                    .FilterByCustomEntityId(query.CustomEntityId)
                    .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                    .SingleOrDefaultAsync();

                result = dbResult?.CustomEntityVersion;
            }

            return result;
        }

        private IEnumerable<string> MapPageRoutings(
            IEnumerable<PageRoutingInfo> allRoutings,
            CustomEntityVersion dbResult
            )
        {
            if (allRoutings == null) return Enumerable.Empty<string>();

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

        #endregion
    }
}
