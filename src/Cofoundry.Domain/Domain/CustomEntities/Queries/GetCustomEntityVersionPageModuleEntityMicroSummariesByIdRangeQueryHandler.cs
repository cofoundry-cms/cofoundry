using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQueryHandler
        : IQueryHandler<GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IAsyncQueryHandler<GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);
            EnforcePermissions(results, executionContext);

            return results;
        }

        public IDictionary<int, RootEntityMicroSummary> Execute(GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = Query(query).ToDictionary(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);
            EnforcePermissions(results, executionContext);

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<ChildEntityMicroSummary> Query(GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersionPageModules
                .AsNoTracking()
                .Where(m => query.CustomEntityVersionPageModuleIds.Contains(m.CustomEntityVersionPageModuleId))
                .Select(m => new ChildEntityMicroSummary()
                {
                    ChildEntityId = m.CustomEntityVersionPageModuleId,
                    RootEntityId = m.CustomEntityVersion.CustomEntityId,
                    RootEntityTitle = m.CustomEntityVersion.Title,
                    EntityDefinitionCode = m.CustomEntityVersion.CustomEntity.CustomEntityDefinition.EntityDefinition.EntityDefinitionCode,
                    EntityDefinitionName = m.CustomEntityVersion.CustomEntity.CustomEntityDefinition.EntityDefinition.Name,
                    IsPreviousVersion = m.CustomEntityVersion.WorkFlowStatusId != (int)WorkFlowStatus.Published || m.CustomEntityVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft
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
