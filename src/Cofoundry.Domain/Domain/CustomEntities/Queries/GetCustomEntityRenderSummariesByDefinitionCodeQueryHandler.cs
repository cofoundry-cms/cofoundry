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
        : IAsyncQueryHandler<GetCustomEntityRenderSummariesByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
        , IPermissionRestrictedQueryHandler<GetCustomEntityRenderSummariesByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
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

        public async Task<IEnumerable<CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query(query).ToListAsync();
            var results = await _customEntityRenderSummaryMapper.MapAsync(dbResults, executionContext);

            return results;
        }

        private IQueryable<CustomEntityVersion> Query(GetCustomEntityRenderSummariesByDefinitionCodeQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Include(e => e.CustomEntity)
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus);

            return dbQuery;
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
