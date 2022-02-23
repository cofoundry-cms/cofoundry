namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to delete a user in custom user areas (excludes the Cofoundry admin user area).
    /// </summary>
    public class NonCofoundryUserDeletePermission : IEntityPermission
    {
        public NonCofoundryUserDeletePermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Non Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}