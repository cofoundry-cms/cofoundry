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
    /// An id range query for custom entities which returns basic
    /// custom entity information with workflow status and model data for the
    /// latest version. The query is not version-sensitive and is designed to be 
    /// used in the admin panel and not in a version-sensitive context such as a 
    /// public webpage.
    /// </summary>
    public class GetCustomEntitySummariesByIdRangeQueryHandler 
        : IQueryHandler<GetCustomEntitySummariesByIdRangeQuery, IDictionary<int, CustomEntitySummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ICustomEntitySummaryMapper _customEntitySummaryMapper;

        public GetCustomEntitySummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPermissionValidationService permissionValidationService,
            ICustomEntitySummaryMapper customEntitySummaryMapper
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
            _customEntitySummaryMapper = customEntitySummaryMapper;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, CustomEntitySummary>> ExecuteAsync(GetCustomEntitySummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await QueryAsync(query, executionContext);

            // Validation permissions
            var definitionCodes = dbResults.Select(r => r.CustomEntity.CustomEntityDefinitionCode);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);

            var mappedResults = await _customEntitySummaryMapper.MapAsync(dbResults, executionContext);

            return mappedResults.ToDictionary(r => r.CustomEntityId);
        }

        #endregion

        #region private helpers

        private async Task<List<CustomEntityPublishStatusQuery>> QueryAsync(GetCustomEntitySummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(e => e.CustomEntityVersion)
                .ThenInclude(e => e.Creator)
                .Include(e => e.CustomEntity)
                .ThenInclude(e => e.Creator)
                .Where(v => query.CustomEntityIds.Contains(v.CustomEntityId))
                .FilterActive()
                .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
                .ToListAsync();
            
            return dbResults;
        }

        #endregion
    }
}
