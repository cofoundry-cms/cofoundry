namespace Cofoundry.Domain;

/// <summary>
/// Read access to image assets. Read access is required in order
/// to include any other image asset permissions.
/// </summary>
public class ImageAssetReadPermission : IEntityPermission
{
    public ImageAssetReadPermission()
    {
        EntityDefinition = new ImageAssetEntityDefinition();
        PermissionType = CommonPermissionTypes.Read("Image Assets");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
