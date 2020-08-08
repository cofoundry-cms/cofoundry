using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetPageEntityMicroSummariesByIdRangeQueryHandler 
        : IQueryHandler<GetPageEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
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
            var results = await Query(query, executionContext).ToDictionaryAsync(e => e.RootEntityId);

            return results;
        }

        private IQueryable<RootEntityMicroSummary> Query(GetPageEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var definition = _entityDefinitionRepository.GetByCode(PageEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .PagePublishStatusQueries
                .AsNoTracking()
                .FilterActive()
                .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
                .Where(q => query.PageIds.Contains(q.PageId))
                .Select(q => q.PageVersion)
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
