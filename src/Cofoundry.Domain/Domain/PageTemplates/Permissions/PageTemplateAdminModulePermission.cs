namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the page templates module in the admin panel.
/// </summary>
public class PageTemplateAdminModulePermission : IEntityPermission
{
    public PageTemplateAdminModulePermission()
    {
        EntityDefinition = new PageTemplateEntityDefinition();
        PermissionType = CommonPermissionTypes.AdminModule("Page Templates");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
