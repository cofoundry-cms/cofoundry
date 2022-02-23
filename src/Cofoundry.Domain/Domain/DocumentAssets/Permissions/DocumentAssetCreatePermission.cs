namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to create new document assets.
    /// </summary>
    public class DocumentAssetCreatePermission : IEntityPermission
    {
        public DocumentAssetCreatePermission()
        {
            EntityDefinition = new DocumentAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Document Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}