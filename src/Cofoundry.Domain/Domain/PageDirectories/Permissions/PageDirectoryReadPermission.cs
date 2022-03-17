namespace Cofoundry.Domain;

/// <summary>
/// Read access to page directories. Read access is required in order
/// to include any other page directory permissions.
/// </summary>
public class PageDirectoryReadPermission : IEntityPermission
{
    public PageDirectoryReadPermission()
    {
        EntityDefinition = new PageDirectoryEntityDefinition();
        PermissionType = CommonPermissionTypes.Read("Page Directories");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
