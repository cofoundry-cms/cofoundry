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
    public class GetPageVersionBlockEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetPageVersionBlockEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetPageVersionBlockEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetPageVersionBlockEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageVersionBlockEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

            return results;
        }

        private IQueryable<ChildEntityMicroSummary> Query(GetPageVersionBlockEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(PageEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .PageVersionBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => query.PageVersionBlockIds.Contains(m.PageVersionBlockId))
                .Select(m => new ChildEntityMicroSummary()
                {
                    ChildEntityId = m.PageVersionBlockId,
                    RootEntityId = m.PageVersion.PageId,
                    RootEntityTitle = m.PageVersion.Title,
                    EntityDefinitionCode = definition.EntityDefinitionCode,
                    EntityDefinitionName = definition.Name,
                    IsPreviousVersion = !m.PageVersion.PagePublishStatusQueries.Any()
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionBlockEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
