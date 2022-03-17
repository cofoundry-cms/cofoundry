namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the image assets module in the admin panel.
/// </summary>
public class ImageAssetAdminModulePermission : IEntityPermission
{
    public ImageAssetAdminModulePermission()
    {
        EntityDefinition = new ImageAssetEntityDefinition();
        PermissionType = CommonPermissionTypes.AdminModule("Image Assets");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
