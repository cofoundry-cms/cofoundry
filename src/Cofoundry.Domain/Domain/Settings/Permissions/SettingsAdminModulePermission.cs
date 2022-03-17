namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the settings module in the admin panel.
/// </summary>
public class SettingsAdminModulePermission : IEntityPermission
{
    public SettingsAdminModulePermission()
    {
        EntityDefinition = new SettingsEntityDefinition();
        PermissionType = CommonPermissionTypes.AdminModule("Settings");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
