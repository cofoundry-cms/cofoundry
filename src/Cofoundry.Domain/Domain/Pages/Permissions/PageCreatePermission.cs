namespace Cofoundry.Domain;

/// <summary>
/// Permission to create new pages.
/// </summary>
public sealed class PageCreatePermission : IEntityPermission
{
    public PageCreatePermission()
    {
        EntityDefinition = new PageEntityDefinition();
        PermissionType = CommonPermissionTypes.Create("Pages");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
