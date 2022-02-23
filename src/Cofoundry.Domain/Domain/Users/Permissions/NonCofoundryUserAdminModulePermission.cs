namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to access the users module in the admin panel for custom user areas (excludes the Cofoundry admin user area).
    /// </summary>
    public class NonCofoundryUserAdminModulePermission : IEntityPermission
    {
        public NonCofoundryUserAdminModulePermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Non Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}