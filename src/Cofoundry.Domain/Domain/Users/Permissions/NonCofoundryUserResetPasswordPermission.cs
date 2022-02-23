using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to force a password reset for users in custom user areas (excludes the Cofoundry admin user area).
    /// </summary>
    public class NonCofoundryUserResetPasswordPermission : IEntityPermission
    {
        public NonCofoundryUserResetPasswordPermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = UserPermissionTypes.ResetPassword;
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}