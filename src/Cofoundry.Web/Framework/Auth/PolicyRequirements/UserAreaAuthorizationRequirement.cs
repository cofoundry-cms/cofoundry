using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web.Auth.Internal;

/// <summary>
/// An <see cref="IAuthorizationRequirement"/> that can be used to construct an
/// authorization policy that restricts access to a specific user area.
/// </summary>
public class UserAreaAuthorizationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new <see cref="UserAreaAuthorizationRequirement"/> instance.
    /// </summary>
    /// <param name="userAreaCode">
    /// The unique 3 character identifier for the user area that the requirement 
    /// should authorize.
    /// </param>
    public UserAreaAuthorizationRequirement(string userAreaCode)
    {
        if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));
        UserAreaCode = userAreaCode;
    }

    /// <summary>
    /// The unique 3 character identifier for the user area that the requirement 
    /// should authorize.
    /// </summary>
    public string UserAreaCode { get; private set; }
}
