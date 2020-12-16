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
    public class GetImageAssetEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetImageAssetEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetImageAssetEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetImageAssetEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetImageAssetEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.RootEntityId);

            return results;
        }

        private IQueryable<RootEntityMicroSummary> Query(GetImageAssetEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(ImageAssetEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .ImageAssets
                .AsNoTracking()
                .FilterByIds(query.ImageAssetIds)
                .Select(v => new RootEntityMicroSummary()
                {
                    RootEntityId = v.ImageAssetId,
                    RootEntityTitle = v.FileName,
                    EntityDefinitionCode = definition.EntityDefinitionCode,
                    EntityDefinitionName = definition.Name
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
