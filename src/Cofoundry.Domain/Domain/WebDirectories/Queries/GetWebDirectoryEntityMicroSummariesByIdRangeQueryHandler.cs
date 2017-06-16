using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetWebDirectoryEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetWebDirectoryEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IAsyncQueryHandler<GetWebDirectoryEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetWebDirectoryEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetWebDirectoryEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetWebDirectoryEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.RootEntityId);

            return results;
        }

        public IDictionary<int, RootEntityMicroSummary> Execute(GetWebDirectoryEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = Query(query).ToDictionary(e => e.RootEntityId);

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<RootEntityMicroSummary> Query(GetWebDirectoryEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(WebDirectoryEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .WebDirectories
                .AsNoTracking()
                .Where(d => query.WebDirectoryIds.Contains(d.WebDirectoryId))
                .Select(d => new RootEntityMicroSummary()
                {
                    RootEntityId = d.WebDirectoryId,
                    RootEntityTitle = d.Name,
                    EntityDefinitionName = definition.Name,
                    EntityDefinitionCode = definition.EntityDefinitionCode
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetWebDirectoryEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new WebDirectoryReadPermission();
        }

        #endregion
    }
}
