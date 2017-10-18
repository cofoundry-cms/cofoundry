using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderDetailsByIdQueryHandler
        : IAsyncQueryHandler<GetCustomEntityRenderDetailsByIdQuery, CustomEntityRenderDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IQueryExecutor _queryExecutor;

        public GetCustomEntityRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            CustomEntityDataModelMapper customEntityDataModelMapper,
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
            var dbResult = await QueryCustomEntity(query).FirstOrDefaultAsync();
            var entity = await MapCustomEntityAsync(dbResult);

            entity.Regions = await QueryRegions(query).ToListAsync();
            var dbPageBlocks = await QueryPageBlocks(entity).ToListAsync();

            var allBlockTypes = await _queryExecutor.GetAllAsync<PageBlockTypeSummary>(executionContext);
            await _entityVersionPageBlockMapper.MapRegionsAsync(dbPageBlocks, entity.Regions, allBlockTypes, query.WorkFlowStatus);

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

            var entity = Mapper.Map<CustomEntityRenderDetails>(dbResult);
            entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);

            return entity;
        }

        private IQueryable<CustomEntityVersion> QueryCustomEntity(GetCustomEntityRenderDetailsByIdQuery query)
        {
            return _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Include(e => e.CustomEntity)
                .ThenInclude(e => e.Locale)
                .FilterByCustomEntityId(query.CustomEntityId)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus, query.CustomEntityVersionId);
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
