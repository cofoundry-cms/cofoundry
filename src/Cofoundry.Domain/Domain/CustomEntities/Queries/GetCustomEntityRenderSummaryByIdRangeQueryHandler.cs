using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummaryByIdRangeQueryHandler 
        : IQueryHandler<GetCustomEntityRenderSummaryByIdRangeQuery, Dictionary<int, CustomEntityRenderSummary>>
        , IAsyncQueryHandler<GetCustomEntityRenderSummaryByIdRangeQuery, Dictionary<int, CustomEntityRenderSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityRenderSummaryByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<Dictionary<int, CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummaryByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query(query).ToListAsync();
            var results = Map(dbResults, executionContext);

            return results;
        }

        public Dictionary<int, CustomEntityRenderSummary> Execute(GetCustomEntityRenderSummaryByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = Query(query).ToList();
            var results = Map(dbResults, executionContext);

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<CustomEntityVersion> Query(GetCustomEntityRenderSummaryByIdRangeQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Where(v => query.CustomEntityIds.Contains(v.CustomEntityId))
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus)
                .Include(e => e.CustomEntity)
                .Include(e => e.CustomEntity.Locale);

            return dbQuery;
        }

        private Dictionary<int, CustomEntityRenderSummary> Map(List<CustomEntityVersion> dbResults, IExecutionContext executionContext)
        {
            var definitionCodes = dbResults.Select(r => r.CustomEntity.CustomEntityDefinitionCode);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);

            var results = new Dictionary<int, CustomEntityRenderSummary>(dbResults.Count);

            foreach (var dbResult in dbResults)
            {
                var entity = Mapper.Map<CustomEntityRenderSummary>(dbResult);
                entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);
                results.Add(entity.CustomEntityId, entity);
            }

            return results;
        }

        #endregion
    }
}
