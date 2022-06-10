namespace Cofoundry.Domain;

public class RoleDefinitionRolePermissionInitializer : IRolePermissionInitializer
{
    private readonly IRoleDefinition _roleDefinition;

    public RoleDefinitionRolePermissionInitializer(IRoleDefinition roleDefinition)
    {
        ArgumentNullException.ThrowIfNull(roleDefinition);

        _roleDefinition = roleDefinition;
    }

    public void Initialize(IPermissionSetBuilder permissionSetBuilder)
    {
        ArgumentNullException.ThrowIfNull(permissionSetBuilder);

        _roleDefinition.ConfigurePermissions(permissionSetBuilder);
    }
}
