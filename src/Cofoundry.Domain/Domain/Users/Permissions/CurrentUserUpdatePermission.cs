namespace Cofoundry.Domain;

/// <summary>
/// Permission to update the currently signed in user account.
/// </summary>
public class CurrentUserUpdatePermission : IEntityPermission
{
    public CurrentUserUpdatePermission()
    {
        EntityDefinition = new CurrentUserEntityDefinition();
        PermissionType = CommonPermissionTypes.Update("Current User");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
