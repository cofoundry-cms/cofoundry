namespace Cofoundry.Domain;

/// <summary>
/// Permission to delete a user in the Cofoundry admin user area.
/// </summary>
public class CofoundryUserDeletePermission : IEntityPermission
{
    public CofoundryUserDeletePermission()
    {
        EntityDefinition = new UserEntityDefinition();
        PermissionType = CommonPermissionTypes.Delete("Cofoundry Users");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
