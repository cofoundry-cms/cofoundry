namespace Cofoundry.Domain;

/// <summary>
/// Permission to publish and unpublish a page.
/// </summary>
public class PagePublishPermission : IEntityPermission
{
    public PagePublishPermission()
    {
        EntityDefinition = new PageEntityDefinition();
        PermissionType = new PermissionType("PAGPUB", "Publish", "Publish or unpublish a page");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
