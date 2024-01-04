namespace Cofoundry.Domain.Data;

public class RolePermission
{
    public int RoleId { get; set; }

    private Role? _role;
    public Role Role
    {
        get => _role ?? throw NavigationPropertyNotInitializedException.Create<RolePermission>(nameof(Role));
        set => _role = value;
    }

    public int PermissionId { get; set; }

    private Permission? _permission;
    public Permission Permission
    {
        get => _permission ?? throw NavigationPropertyNotInitializedException.Create<RolePermission>(nameof(Permission));
        set => _permission = value;
    }
}
