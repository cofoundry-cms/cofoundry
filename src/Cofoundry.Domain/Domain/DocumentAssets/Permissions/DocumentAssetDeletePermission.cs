namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to delete a document asset.
    /// </summary>
    public class DocumentAssetDeletePermission : IEntityPermission
    {
        public DocumentAssetDeletePermission()
        {
            EntityDefinition = new DocumentAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Document Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}