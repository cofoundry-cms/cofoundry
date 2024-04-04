namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns all IPermission instances registered with Cofoundry.
/// </summary>
public class GetAllPermissionsQueryHandler
    : IQueryHandler<GetAllPermissionsQuery, IReadOnlyCollection<IPermission>>
    , IPermissionRestrictedQueryHandler<GetAllPermissionsQuery, IReadOnlyCollection<IPermission>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetAllPermissionsQueryHandler(
        IPermissionRepository permissionRepository
        )
    {
        _permissionRepository = permissionRepository;
    }

    public Task<IReadOnlyCollection<IPermission>> ExecuteAsync(GetAllPermissionsQuery query, IExecutionContext executionContext)
    {
        var permissions = _permissionRepository
            .GetAll()
            .OrderBy(GetPrimaryOrdering)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<IPermission>>(permissions);
    }

    private string GetPrimaryOrdering(IPermission permission)
    {
        if (permission is IEntityPermission entityPermission)
        {
            return entityPermission.EntityDefinition.Name;
        }

        return "ZZZ";
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPermissionsQuery query)
    {
        yield return new RoleReadPermission();
    }
}