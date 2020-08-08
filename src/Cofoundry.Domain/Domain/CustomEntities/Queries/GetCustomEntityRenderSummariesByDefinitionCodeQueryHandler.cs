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
    /// Query to retreive all custom entites of a specific type, projected as a
    /// CustomEntityRenderSummary, which is a general-purpose projection with version 
    /// specific data, including a deserialized data model. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.    
    /// </summary>
    public class GetCustomEntityRenderSummariesByDefinitionCodeQueryHandler
        : IQueryHandler<GetCustomEntityRenderSummariesByDefinitionCodeQuery, ICollection<CustomEntityRenderSummary>>
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
                .FilterActive()
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode)
                .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                .ToListAsync();

            // EF doesn't allow includes after selects, so re-filter the results.

            return dbResults
                .Select(e => e.CustomEntityVersion)
                .ToList();
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityRenderSummariesByDefinitionCodeQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
