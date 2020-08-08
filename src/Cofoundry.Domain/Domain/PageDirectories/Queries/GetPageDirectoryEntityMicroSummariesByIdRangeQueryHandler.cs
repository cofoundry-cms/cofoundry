using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetPageDirectoryEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetPageDirectoryEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetPageDirectoryEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetPageDirectoryEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution
        
        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageDirectoryEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.RootEntityId);

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<RootEntityMicroSummary> Query(GetPageDirectoryEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(PageDirectoryEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(d => query.PageDirectoryIds.Contains(d.PageDirectoryId))
                .Select(d => new RootEntityMicroSummary()
                {
                    RootEntityId = d.PageDirectoryId,
                    RootEntityTitle = d.Name,
                    EntityDefinitionName = definition.Name,
                    EntityDefinitionCode = definition.EntityDefinitionCode
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
