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
    public class GetCustomEntitySummaryByIdRangeQueryHandler 
        : IAsyncQueryHandler<GetByIdRangeQuery<CustomEntitySummary>, IDictionary<int, CustomEntitySummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ICustomEntitySummaryMapper _customEntitySummaryMapper;

        public GetCustomEntitySummaryByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPermissionValidationService permissionValidationService,
            ICustomEntitySummaryMapper customEntitySummaryMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _permissionValidationService = permissionValidationService;
            _customEntitySummaryMapper = customEntitySummaryMapper;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, CustomEntitySummary>> ExecuteAsync(GetByIdRangeQuery<CustomEntitySummary> query, IExecutionContext executionContext)
        {
            var dbResults = await QueryAsync(query.Ids, executionContext);

            // Validation permissions
            var definitionCodes = dbResults.Select(r => r.CustomEntity.CustomEntityDefinitionCode);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);

            var mappedResults = await _customEntitySummaryMapper.MapAsync(dbResults, executionContext);

            return mappedResults.ToDictionary(r => r.CustomEntityId);
        }

        #endregion

        #region private helpers

        private async Task<List<CustomEntityPublishStatusQuery>> QueryAsync(int[] ids, IExecutionContext executionContext)
        {
            var dbResults = await _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(e => e.CustomEntityVersion)
                .ThenInclude(e => e.Creator)
                .Include(e => e.CustomEntity)
                .ThenInclude(e => e.Locale)
                .Include(e => e.CustomEntity)
                .ThenInclude(e => e.Creator)
                .Where(v => ids.Contains(v.CustomEntityId))
                .FilterByActive()
                .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
                .ToListAsync();
            
            return dbResults;
        }

        #endregion
    }
}
