namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to delete an image asset.
    /// </summary>
    public class ImageAssetDeletePermission : IEntityPermission
    {
        public ImageAssetDeletePermission()
        {
            EntityDefinition = new ImageAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Image Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}