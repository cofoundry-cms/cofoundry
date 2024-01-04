using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetCustomEntityVersionEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPermissionValidationService _permissionValidationService;

    public GetCustomEntityVersionEntityMicroSummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IPermissionValidationService permissionValidationService
        )
    {
        _dbContext = dbContext;
        _permissionValidationService = permissionValidationService;
    }

    public async Task<IReadOnlyDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var results = await _dbContext
            .CustomEntityVersions
            .AsNoTracking()
            .Where(v => query.CustomEntityVersionIds.Contains(v.CustomEntityVersionId))
            .Select(v => new ChildEntityMicroSummary()
            {
                ChildEntityId = v.CustomEntityVersionId,
                RootEntityId = v.CustomEntityId,
                RootEntityTitle = v.Title,
                EntityDefinitionName = v.CustomEntity.CustomEntityDefinition.EntityDefinition.Name,
                EntityDefinitionCode = v.CustomEntity.CustomEntityDefinition.EntityDefinition.EntityDefinitionCode,
                IsPreviousVersion = !v.CustomEntityPublishStatusQueries.Any()
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