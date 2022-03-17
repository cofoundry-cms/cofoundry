namespace Cofoundry.Domain;

/// <summary>
/// Permission to update the url of a page directory.
/// </summary>
public class PageDirectoryUpdateUrlPermission : IEntityPermission
{
    public PageDirectoryUpdateUrlPermission()
    {
        EntityDefinition = new PageDirectoryEntityDefinition();
        PermissionType = new PermissionType("UPDURL", "Update Page Directory Url", "Update the url of a page directory");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
