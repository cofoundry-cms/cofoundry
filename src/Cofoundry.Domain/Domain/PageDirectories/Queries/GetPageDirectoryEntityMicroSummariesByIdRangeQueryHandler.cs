using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class GetPageDirectoryEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetPageDirectoryEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetPageDirectoryEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
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

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageDirectoryEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var definition = _entityDefinitionRepository.GetByCode(PageDirectoryEntityDefinition.DefinitionCode);

            var results = await _dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(d => query.PageDirectoryIds.Contains(d.PageDirectoryId))
                .Select(d => new RootEntityMicroSummary()
                {
                    RootEntityId = d.PageDirectoryId,
                    RootEntityTitle = d.Name,
                    EntityDefinitionName = definition.Name,
                    EntityDefinitionCode = definition.EntityDefinitionCode
                })
                .ToDictionaryAsync(e => e.RootEntityId);

            return results;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new PageDirectoryReadPermission();
        }
    }
}
