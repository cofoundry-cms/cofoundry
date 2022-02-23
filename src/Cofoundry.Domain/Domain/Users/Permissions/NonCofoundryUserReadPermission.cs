namespace Cofoundry.Domain
{
    /// <summary>
    /// Read access to users in custom user areas (excludes the Cofoundry admin user area). Read access is 
    /// required in order to include any other permissions.
    /// </summary>
    public class NonCofoundryUserReadPermission : IEntityPermission
    {
        public NonCofoundryUserReadPermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Non Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}