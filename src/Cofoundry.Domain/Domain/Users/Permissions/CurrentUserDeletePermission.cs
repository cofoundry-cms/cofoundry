namespace Cofoundry.Domain;

/// <summary>
/// Permission to delete the currently signed in user account.
/// </summary>
public class CurrentUserDeletePermission : IEntityPermission
{
    public CurrentUserDeletePermission()
    {
        EntityDefinition = new CurrentUserEntityDefinition();
        PermissionType = CommonPermissionTypes.Delete("Current User");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
