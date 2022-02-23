namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update users in custom user areas (excludes the Cofoundry admin user area), but not actions
    /// covered by special permissions such as issuing a password reset.
    /// </summary>
    public class NonCofoundryUserUpdatePermission : IEntityPermission
    {
        public NonCofoundryUserUpdatePermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Non Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}