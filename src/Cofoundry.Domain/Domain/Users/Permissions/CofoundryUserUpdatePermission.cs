namespace Cofoundry.Domain;

/// <summary>
/// Permission to update users in the Cofoundry admin user area, but not actions
/// covered by special permissions such as issuing a password reset.
/// </summary>
public class CofoundryUserUpdatePermission : IEntityPermission
{
    public CofoundryUserUpdatePermission()
    {
        EntityDefinition = new UserEntityDefinition();
        PermissionType = CommonPermissionTypes.Update("Cofoundry Users");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
