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
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityRenderSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await QueryAsync(query, executionContext);

            EnforcePermissions(dbResults, executionContext);
            var results = await _customEntityRenderSummaryMapper.MapAsync(dbResults, executionContext);

            return results.ToDictionary(r => r.CustomEntityId);
        }

        private async Task<List<CustomEntityVersion>> QueryAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
            {
                throw new InvalidOperationException("PublishStatusQuery.SpecificVersion not supported in GetCustomEntityRenderSummariesByDefinitionCodeQuery");
            }

            var dbQuery = await _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(e => e.CustomEntityVersion)
                .ThenInclude(e => e.CustomEntity)
                .FilterByActive()
                .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                .Where(v => query.CustomEntityIds.Contains(v.CustomEntityId))
                .ToListAsync();

            return dbQuery
                .Select(v => v.CustomEntityVersion)
                .ToList();
        }

        private void EnforcePermissions(List<CustomEntityVersion> dbResults, IExecutionContext executionContext)
        {
            var definitionCodes = dbResults.Select(r => r.CustomEntity.CustomEntityDefinitionCode);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);
        }

        #endregion
    }
}
