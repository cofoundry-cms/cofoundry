namespace Cofoundry.Domain
{
    /// <summary>
    /// Read access to all users in the Cofoundry admin user area. Read access is 
    /// required in order to include any other Cofoundry user permissions.
    /// </summary>
    public class CofoundryUserReadPermission : IEntityPermission
    {
        public CofoundryUserReadPermission()
        {
            EntityDefinition = new UserEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}