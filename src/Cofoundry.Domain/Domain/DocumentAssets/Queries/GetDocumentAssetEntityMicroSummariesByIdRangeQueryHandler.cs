using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetDocumentAssetEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetDocumentAssetEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetDocumentAssetEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetDocumentAssetEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetDocumentAssetEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.RootEntityId);

            return results;
        }

        private IQueryable<RootEntityMicroSummary> Query(GetDocumentAssetEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(DocumentAssetEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .DocumentAssets
                .AsNoTracking()
                .FilterByIds(query.DocumentAssetIds)
                .Select(a => new RootEntityMicroSummary()
                {
                    RootEntityId = a.DocumentAssetId,
                    RootEntityTitle = a.Title,
                    EntityDefinitionCode = definition.EntityDefinitionCode,
                    EntityDefinitionName = definition.Name
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
