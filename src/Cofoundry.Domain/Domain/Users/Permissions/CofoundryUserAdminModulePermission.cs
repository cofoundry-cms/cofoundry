namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to access the Cofoundry users module in the admin panel.
    /// </summary>
    public class CofoundryUserAdminModulePermission : IEntityPermission
    {
        public CofoundryUserAdminModulePermission()
        {
            EntityDefinition = new UserEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}