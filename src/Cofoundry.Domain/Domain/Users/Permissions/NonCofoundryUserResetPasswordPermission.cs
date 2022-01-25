using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
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