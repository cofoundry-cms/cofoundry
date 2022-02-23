namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to create new image assets.
    /// </summary>
    public class ImageAssetCreatePermission : IEntityPermission
    {
        public ImageAssetCreatePermission()
        {
            EntityDefinition = new ImageAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Image Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}