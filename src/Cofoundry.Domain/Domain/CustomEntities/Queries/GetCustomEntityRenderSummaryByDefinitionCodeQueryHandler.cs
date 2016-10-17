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
    public class GetCustomEntityRenderSummaryByDefinitionCodeQueryHandler 
        : IQueryHandler<GetCustomEntityRenderSummaryByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
        , IAsyncQueryHandler<GetCustomEntityRenderSummaryByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
        , IPermissionRestrictedQueryHandler<GetCustomEntityRenderSummaryByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityRenderSummaryByDefinitionCodeQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IEnumerable<CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummaryByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query(query).ToListAsync();
            var results = await _customEntityRenderSummaryMapper.MapSummariesAsync(dbResults, executionContext);

            return results;
        }

        public IEnumerable<CustomEntityRenderSummary> Execute(GetCustomEntityRenderSummaryByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var dbResults = Query(query).ToList();
            var results = _customEntityRenderSummaryMapper.MapSummaries(dbResults, executionContext);

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<CustomEntityVersion> Query(GetCustomEntityRenderSummaryByDefinitionCodeQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus)
                .Include(e => e.CustomEntity);

            return dbQuery;
        }
        
        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityRenderSummaryByDefinitionCodeQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
