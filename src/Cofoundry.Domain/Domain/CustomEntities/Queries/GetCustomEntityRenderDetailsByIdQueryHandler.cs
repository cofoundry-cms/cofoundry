using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;
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
        private readonly IEntityVersionPageModuleMapper _entityVersionPageModuleMapper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IQueryExecutor _queryExecutor;

        public GetCustomEntityRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            IEntityVersionPageModuleMapper entityVersionPageModuleMapper,
            IPermissionValidationService permissionValidationService,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _entityVersionPageModuleMapper = entityVersionPageModuleMapper;
            _permissionValidationService = permissionValidationService;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task<CustomEntityRenderDetails> ExecuteAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryCustomEntity(query).FirstOrDefaultAsync();
            var entity = await MapCustomEntityAsync(dbResult);

            entity.Sections = await QuerySections(query).ToListAsync();
            var dbModules = await QueryModules(entity).ToListAsync();

            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);
            _entityVersionPageModuleMapper.MapSections(dbModules, entity.Sections, allModuleTypes, query.WorkFlowStatus);

            var routingQuery = new GetPageRoutingInfoByCustomEntityIdQuery(dbResult.CustomEntityId);
            var routing = await _queryExecutor.ExecuteAsync(routingQuery, executionContext);
            entity.DetailsPageUrls = MapPageRoutings(routing, dbResult);

            return entity;
        }

        private IQueryable<CustomEntityVersionPageModule> QueryModules(CustomEntityRenderDetails entity)
        {
            return _dbContext
                .CustomEntityVersionPageModules
                .AsNoTracking()
                .Where(m => m.CustomEntityVersionId == entity.CustomEntityVersionId);
        }

        private IQueryable<CustomEntityPageSectionRenderDetails> QuerySections(GetCustomEntityRenderDetailsByIdQuery query)
        {
            return _dbContext
                .PageTemplateSections
                .AsNoTracking()
                .Where(s => s.PageTemplateId == query.PageTemplateId)
                .ProjectTo<CustomEntityPageSectionRenderDetails>();
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
                .FilterByCustomEntityId(query.CustomEntityId)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus, query.CustomEntityVersionId)
                .Include(e => e.CustomEntity)
                .Include(e => e.CustomEntity.Locale);
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
