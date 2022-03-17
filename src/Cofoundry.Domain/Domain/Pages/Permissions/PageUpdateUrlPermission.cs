namespace Cofoundry.Domain;

/// <summary>
/// Permission to update the url of a page.
/// </summary>
public class PageUpdateUrlPermission : IEntityPermission
{
    public PageUpdateUrlPermission()
    {
        EntityDefinition = new PageEntityDefinition();
        PermissionType = new PermissionType("UPDURL", "Update Page Url", "Update the url of a page");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
