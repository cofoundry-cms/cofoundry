namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to access the document assets module in the admin panel.
    /// </summary>
    public class DocumentAssetAdminModulePermission : IEntityPermission
    {
        public DocumentAssetAdminModulePermission()
        {
            EntityDefinition = new DocumentAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Document Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}