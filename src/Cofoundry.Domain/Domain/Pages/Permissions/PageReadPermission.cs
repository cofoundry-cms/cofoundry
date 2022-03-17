namespace Cofoundry.Domain;

/// <summary>
/// Read access to pages. Read access is required in order
/// to include any other page permissions.
/// </summary>
public sealed class PageReadPermission : IEntityPermission
{
    public IEntityDefinition EntityDefinition => new PageEntityDefinition();

    public PermissionType PermissionType => CommonPermissionTypes.Read("Pages");
}
