namespace Cofoundry.Domain;

/// <summary>
/// Read access to roles. Read access is required in order
/// to include any other role permissions.
/// </summary>
public class RoleReadPermission : IEntityPermission
{
    public RoleReadPermission()
    {
        EntityDefinition = new RoleEntityDefinition();
        PermissionType = CommonPermissionTypes.Read("Roles");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
