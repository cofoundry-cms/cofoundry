using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    public class CofoundryUserResetPasswordPermission : IEntityPermission
    {
        public CofoundryUserResetPasswordPermission()
        {
            EntityDefinition = new UserEntityDefinition();
            PermissionType = UserPermissionTypes.ResetPassword;
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}