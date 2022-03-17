namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the pages module in the admin panel.
/// </summary>
public sealed class PageAdminModulePermission : IEntityPermission
{
    public PageAdminModulePermission()
    {
        EntityDefinition = new PageEntityDefinition();
        PermissionType = CommonPermissionTypes.AdminModule("Pages");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
