namespace Cofoundry.Domain;

/// <summary>
/// Permission to update an image asset.
/// </summary>
public class ImageAssetUpdatePermission : IEntityPermission
{
    public ImageAssetUpdatePermission()
    {
        EntityDefinition = new ImageAssetEntityDefinition();
        PermissionType = CommonPermissionTypes.Update("Image Assets");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
