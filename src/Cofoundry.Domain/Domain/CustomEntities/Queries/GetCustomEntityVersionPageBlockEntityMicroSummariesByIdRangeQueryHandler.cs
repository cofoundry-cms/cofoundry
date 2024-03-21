﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
    , IIgnorePermissionCheckHandler
{
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

    public async Task<IReadOnlyDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var results = await _dbContext
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
                IsPreviousVersion = m.CustomEntityVersion.CustomEntityPublishStatusQueries.Count == 0
            })
            .ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

        EnforcePermissions(results, executionContext);

        return results;
    }

    private void EnforcePermissions(IDictionary<int, RootEntityMicroSummary> entities, IExecutionContext executionContext)
    {
        var definitionCodes = entities.Select(e => e.Value.EntityDefinitionCode);

        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);
    }
}