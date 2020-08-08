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
    public class GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);
            EnforcePermissions(results, executionContext);

            return results;
        }

        private IQueryable<ChildEntityMicroSummary> Query(GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersionPageBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => query.CustomEntityVersionPageBlockIds.Contains(m.CustomEntityVersionPageBlockId))
                .Select(m => new ChildEntityMicroSummary()
                {
                    ChildEntityId = m.CustomEntityVersionPageBlockId,
                    RootEntityId = m.CustomEntityVersion.CustomEntityId,
                    RootEntityTitle = m.CustomEntityVersion.Title,
                    EntityDefinitionCode = m.CustomEntityVersion.CustomEntity.CustomEntityDefinition.EntityDefinition.EntityDefinitionCode,
                    EntityDefinitionName = m.CustomEntityVersion.CustomEntity.CustomEntityDefinition.EntityDefinition.Name,
                    IsPreviousVersion = !m.CustomEntityVersion.CustomEntityPublishStatusQueries.Any()
                });

            return dbQuery;
        }

        private void EnforcePermissions(IDictionary<int, RootEntityMicroSummary> entities, IExecutionContext executionContext)
        {
            var definitionCodes = entities.Select(e => e.Value.EntityDefinitionCode);

            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);
        }

        #endregion
    }
}
