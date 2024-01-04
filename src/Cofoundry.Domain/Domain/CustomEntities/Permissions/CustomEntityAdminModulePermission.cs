namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the custom entity module in the admin panel.
/// </summary>
public class CustomEntityAdminModulePermission : ICustomEntityPermissionTemplate
{
    /// <summary>
    /// Constructor used internally by AuthorizePermissionAttribute.
    /// </summary>
    public CustomEntityAdminModulePermission()
    {
        // Invalid/uninitialized data but should only be used by AuthorizePermissionAttribute
        PermissionType = CommonPermissionTypes.AdminModule("Not Set");
        EntityDefinition = new CustomEntityDynamicEntityDefinition();
    }

    public CustomEntityAdminModulePermission(ICustomEntityDefinition customEntityDefinition)
    {
        EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
        PermissionType = CommonPermissionTypes.AdminModule(customEntityDefinition.NamePlural);
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }

    public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
    {
        var implementedPermission = new CustomEntityAdminModulePermission(customEntityDefinition);
        return implementedPermission;
    }
}
