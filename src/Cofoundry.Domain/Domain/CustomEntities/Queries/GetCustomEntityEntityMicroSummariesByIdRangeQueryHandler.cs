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
    public class GetCustomEntityEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetCustomEntityEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetCustomEntityEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query, executionContext).ToDictionaryAsync(e => e.RootEntityId);
            EnforcePermissions(results, executionContext);

            return results;
        }

        private IQueryable<RootEntityMicroSummary> Query(GetCustomEntityEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbQuery = _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .FilterActive()
                .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
                .Where(v => query.CustomEntityIds.Contains(v.CustomEntityId))
                .Select(v => new RootEntityMicroSummary()
                {
                    RootEntityId = v.CustomEntityId,
                    RootEntityTitle = v.CustomEntityVersion.Title,
                    EntityDefinitionName = v.CustomEntity.CustomEntityDefinition.EntityDefinition.Name,
                    EntityDefinitionCode = v.CustomEntity.CustomEntityDefinition.CustomEntityDefinitionCode
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
