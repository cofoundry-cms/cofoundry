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
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityRenderSummaryByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public Dictionary<int, CustomEntityRenderSummary> Execute(GetCustomEntityRenderSummaryByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = Query(query).ToList();
            EnforcePermissions(dbResults, executionContext);
            var results = _customEntityRenderSummaryMapper.MapSummaries(dbResults, executionContext);

            return results.ToDictionary(r => r.CustomEntityId);
        }

        public async Task<Dictionary<int, CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummaryByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query(query).ToListAsync();
            EnforcePermissions(dbResults, executionContext);
            var results = await _customEntityRenderSummaryMapper.MapSummariesAsync(dbResults, executionContext);

            return results.ToDictionary(r => r.CustomEntityId);
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

        private void EnforcePermissions(List<CustomEntityVersion> dbResults, IExecutionContext executionContext)
        {
            var definitionCodes = dbResults.Select(r => r.CustomEntity.CustomEntityDefinitionCode);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);
        }

        #endregion
    }
}
