namespace Cofoundry.Domain;

/// <summary>
/// Permission to create new users in the Cofoundry admin user area.
/// </summary>
public class CofoundryUserCreatePermission : IEntityPermission
{
    public CofoundryUserCreatePermission()
    {
        EntityDefinition = new UserEntityDefinition();
        PermissionType = CommonPermissionTypes.Create("Cofoundry Users");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
