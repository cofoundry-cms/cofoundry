using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web.Auth.Internal;

/// <summary>
/// An <see cref="IAuthorizationRequirement"/> that can be used to construct an
/// authorization policy that restricts access to users with the assigned permission.
/// If a user is not logged in, then the anonymous role will be checked to validate
/// the permission.
/// </summary>
public class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new <see cref="PermissionAuthorizationRequirement"/> instance.
    /// </summary>
    /// <param name="permission">
    /// The permission to restrict access to.
    /// </param>
    public PermissionAuthorizationRequirement(IPermission permission)
    {
        if (permission == null) throw new ArgumentNullException(nameof(permission));
        Permission = permission;
    }

    /// <summary>
    /// The permission to restrict access to.
    /// </summary>
    public IPermission Permission { get; private set; }
}
