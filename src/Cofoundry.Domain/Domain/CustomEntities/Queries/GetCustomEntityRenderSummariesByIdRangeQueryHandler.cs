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
    public class GetCustomEntityRenderSummariesByIdRangeQueryHandler
        : IAsyncQueryHandler<GetCustomEntityRenderSummariesByIdRangeQuery, IDictionary<int, CustomEntityRenderSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityRenderSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityDataModelMapper customEntityDataModelMapper,
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

        public async Task<IDictionary<int, CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query(query).ToListAsync();
            EnforcePermissions(dbResults, executionContext);
            var results = await _customEntityRenderSummaryMapper.MapAsync(dbResults, executionContext);

            return results.ToDictionary(r => r.CustomEntityId);
        }

        private IQueryable<CustomEntityVersion> Query(GetCustomEntityRenderSummariesByIdRangeQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Include(e => e.CustomEntity)
                .ThenInclude(e => e.Locale)
                .Where(v => query.CustomEntityIds.Contains(v.CustomEntityId))
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus);

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
