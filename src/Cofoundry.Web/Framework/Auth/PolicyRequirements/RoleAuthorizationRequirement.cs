using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web.Auth.Internal;

/// <summary>
/// An <see cref="IAuthorizationRequirement"/> that can be used to construct an
/// authorization policy that restricts access to a one or more Cofoundry roles. This
/// shouldn't be confused with ASP.NET Identity roles which are different to
/// Cofoundry Roles.
/// </summary>
public class RoleAuthorizationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new <see cref="RoleAuthorizationRequirement"/> instance.
    /// </summary>
    /// <param name="userAreaCode">
    /// The unique 3 charcter code identifier for the user area that the role belongs to.
    /// </param>
    /// <param name="roleCode">
    /// The unique 3 character identifier for the role that the requirement 
    /// should authorize.
    /// </param>
    public RoleAuthorizationRequirement(string userAreaCode, string roleCode)
        : this(userAreaCode, new string[] { roleCode })
    {
        if (string.IsNullOrWhiteSpace(roleCode)) throw new ArgumentEmptyException(nameof(roleCode));
    }

    /// <summary>
    /// Initializes a new <see cref="RoleAuthorizationRequirement"/> instance.
    /// </summary>
    /// <param name="userAreaCode">
    /// The unique 3 charcter code identifier for the user area that the roles belong to.
    /// </param>
    /// <param name="roleCodes">
    /// Collection of role code identifiers to authorize against. To pass authorization the
    /// user should be assigned one of the roles in this collection. Each of the roles must
    /// belong to the user area passed in <paramref name="roleCodes"/>.
    /// </param>
    public RoleAuthorizationRequirement(string userAreaCode, IEnumerable<string> roleCodes)
    {
        if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));
        if (roleCodes == null) throw new ArgumentNullException(nameof(roleCodes));
        if (roleCodes.Count() == 0) throw new ArgumentOutOfRangeException(nameof(roleCodes));

        UserAreaCode = userAreaCode;
        RoleCodes = roleCodes;
    }

    /// <summary>
    /// The unique 3 charcter code identifier for the user area that the roles belong to.
    /// </summary>
    public string UserAreaCode { get; private set; }

    /// <summary>
    /// Collection of role code identifiers to authorize against. To pass authorization the
    /// user should be assigned one of the roles in this collection. Each of the roles must
    /// belong to the user area defined in <see cref="UserAreaCode"/>.
    /// </summary>
    public IEnumerable<string> RoleCodes { get; private set; }
}
