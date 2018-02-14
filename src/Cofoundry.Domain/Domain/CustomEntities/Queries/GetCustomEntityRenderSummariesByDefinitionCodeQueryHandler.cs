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
    public class GetCustomEntityRenderSummariesByDefinitionCodeQueryHandler
        : IAsyncQueryHandler<GetCustomEntityRenderSummariesByDefinitionCodeQuery, ICollection<CustomEntityRenderSummary>>
        , IPermissionRestrictedQueryHandler<GetCustomEntityRenderSummariesByDefinitionCodeQuery, ICollection<CustomEntityRenderSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityRenderSummariesByDefinitionCodeQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<ICollection<CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await QueryAsync(query, executionContext);
            var results = await _customEntityRenderSummaryMapper.MapAsync(dbResults, executionContext);

            return results;
        }

        private async Task<List<CustomEntityVersion>> QueryAsync(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
            {
                throw new InvalidOperationException("PublishStatusQuery.SpecificVersion not supported in GetCustomEntityRenderSummariesByDefinitionCodeQuery");
            }

            var dbResults = await _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(e => e.CustomEntityVersion)
                .ThenInclude(e => e.CustomEntity)
                .FilterByActive()
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode)
                .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                .ToListAsync();

            // EF doesn't allow includes after selects, so re-filter the results.

            return dbResults
                .Select(e => e.CustomEntityVersion)
                .ToList();
        }
        
        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityRenderSummariesByDefinitionCodeQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
