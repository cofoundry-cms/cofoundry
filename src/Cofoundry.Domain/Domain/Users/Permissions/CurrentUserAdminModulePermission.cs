namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the current user account module in the admin panel. This only applied
/// to the current user in the Cofoundry admnin user area and not to any custom user areas.
/// </summary>
public class CurrentUserAdminModulePermission : IEntityPermission
{
    public CurrentUserAdminModulePermission()
    {
        EntityDefinition = new CurrentUserEntityDefinition();
        PermissionType = CommonPermissionTypes.AdminModule("Current User Account");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
