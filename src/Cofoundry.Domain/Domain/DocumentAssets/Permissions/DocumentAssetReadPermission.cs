namespace Cofoundry.Domain;

/// <summary>
/// Read access to document assets. Read access is required in order
/// to include any other document asset permissions.
/// </summary>
public class DocumentAssetReadPermission : IEntityPermission
{
    public DocumentAssetReadPermission()
    {
        EntityDefinition = new DocumentAssetEntityDefinition();
        PermissionType = CommonPermissionTypes.Read("Document Assets");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
