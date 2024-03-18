namespace Cofoundry.Domain.Data;

/// <summary>
/// Roles are an assignable collection of permissions. Every user has to be assigned 
/// to one role.
/// </summary>
public class Role
{
    /// <summary>
    /// Database id of the role.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// The role title is used to identify the role and select it in the admin 
    /// UI and therefore must be unique. Max 50 characters.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The role code is a unique three letter code that can be used to reference the role 
    /// programatically. The code must be unique and convention is to use upper case, although 
    /// code matching is case insensitive. This is only used by roles defined in code using 
    /// <see cref="IRoleDefinition"/>.
    /// </summary>
    public string? RoleCode { get; set; }

    /// <summary>
    /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
    /// </summary>
    public string UserAreaCode { get; set; } = string.Empty;

    private UserArea? _userArea;
    /// <summary>
    /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
    /// </summary>
    public UserArea UserArea
    {
        get => _userArea ?? throw NavigationPropertyNotInitializedException.Create<Role>(nameof(UserArea));
        set => _userArea = value;
    }

    /// <summary>
    /// Collection of permissions that describe the actions this role is 
    /// permitted to perform.
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Dynamic website routes can optionally be restircted to specific roles. This
    /// collection references zero or more access rules at the <see cref="Page"/> level.
    /// </summary>
    public ICollection<PageAccessRule> PageAccessRules { get; set; } = new List<PageAccessRule>();

    /// <summary>
    /// Dynamic website routes can optionally be restircted to specific roles. This
    /// collection references zero or more access rules at the <see cref="PageDirectory"/> level.
    /// </summary>
    public ICollection<PageDirectoryAccessRule> PageDirectoryAccessRules { get; set; } = new List<PageDirectoryAccessRule>();

    /// <summary>
    /// <see langword="true"/> if this role is the special <see cref="AnonymousRole"/>.
    /// </summary>
    public bool IsAnonymousRole()
    {
        return RoleCode == AnonymousRole.Code && UserAreaCode == CofoundryAdminUserArea.Code;
    }

    /// <summary>
    /// <see langword="true"/> if this role is the special Cofoundry <see cref="SuperAdminRole"/>.
    /// </summary>
    public bool IsSuperAdminRole()
    {
        return RoleCode == SuperAdminRole.Code && UserAreaCode == CofoundryAdminUserArea.Code;
    }
}
