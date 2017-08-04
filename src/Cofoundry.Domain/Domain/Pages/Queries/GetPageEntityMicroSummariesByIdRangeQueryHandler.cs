using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetPageEntityMicroSummariesByIdRangeQueryHandler 
        : IAsyncQueryHandler<GetPageEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetPageEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetPageEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.RootEntityId);

            return results;
        }

        private IQueryable<RootEntityMicroSummary> Query(GetPageEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(PageEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .PageVersions
                .AsNoTracking()
                .FilterByWorkFlowStatusQuery(WorkFlowStatusQuery.Latest)
                .Where(v => query.PageIds.Contains(v.PageId))
                .Select(v => new RootEntityMicroSummary()
                {
                    RootEntityId = v.PageId,
                    RootEntityTitle = v.Title,
                    EntityDefinitionName = definition.Name,
                    EntityDefinitionCode = definition.EntityDefinitionCode
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
