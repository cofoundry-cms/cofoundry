using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Permission to force a password reset for a user in the Cofoundry admin user area.
/// </summary>
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
