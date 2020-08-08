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
    public class GetPageVersionEntityMicroSummariesByIdRangeQueryHandler 
        : IQueryHandler<GetPageVersionEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetPageVersionEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetPageVersionEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageVersionEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

            return results;
        }

        private IQueryable<ChildEntityMicroSummary> Query(GetPageVersionEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(PageEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(v => query.PageVersionIds.Contains(v.PageVersionId))
                .Select(v => new ChildEntityMicroSummary()
                {
                    ChildEntityId = v.PageVersionId,
                    RootEntityId = v.PageId,
                    RootEntityTitle = v.Title,
                    EntityDefinitionCode = definition.EntityDefinitionCode,
                    EntityDefinitionName = definition.Name,
                    IsPreviousVersion = !v.PagePublishStatusQueries.Any() // not draft or latest published version
                });
            
            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
