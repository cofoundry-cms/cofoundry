namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to create new users in custom user areas (excludes the Cofoundry admin user area).
    /// </summary>
    public class NonCofoundryUserCreatePermission : IEntityPermission
    {
        public NonCofoundryUserCreatePermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Non Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}