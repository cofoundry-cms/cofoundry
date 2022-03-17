namespace Cofoundry.Domain;

public class RoleDefinitionRolePermissionInitializer : IRolePermissionInitializer
{
    private readonly IRoleDefinition _roleDefinition;

    public RoleDefinitionRolePermissionInitializer(IRoleDefinition roleDefinition)
    {
        if (roleDefinition == null) throw new ArgumentNullException(nameof(roleDefinition));

        _roleDefinition = roleDefinition;
    }

    public void Initialize(IPermissionSetBuilder permissionSetBuilder)
    {
        if (permissionSetBuilder == null) throw new ArgumentNullException(nameof(permissionSetBuilder));

        _roleDefinition.ConfigurePermissions(permissionSetBuilder);
    }
}
