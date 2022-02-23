namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update a document asset.
    /// </summary>
    public class DocumentAssetUpdatePermission : IEntityPermission
    {
        public DocumentAssetUpdatePermission()
        {
            EntityDefinition = new DocumentAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Document Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}