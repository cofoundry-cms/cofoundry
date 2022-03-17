namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the page directories module in the admin panel.
/// </summary>
public class PageDirectoryAdminModulePermission : IEntityPermission
{
    public PageDirectoryAdminModulePermission()
    {
        EntityDefinition = new PageDirectoryEntityDefinition();
        PermissionType = CommonPermissionTypes.AdminModule("Page Directories");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
