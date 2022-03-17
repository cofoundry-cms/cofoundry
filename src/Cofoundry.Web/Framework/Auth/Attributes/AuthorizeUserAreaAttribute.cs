using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web;

/// <summary>
/// Ensures that a class or method can only be accessed by users associated with 
/// a specific user area. Using this attribute will set the current user context 
/// to the specified user area, rather than the default.
/// </summary>
public class AuthorizeUserAreaAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeUserAreaAttribute"/> class.
    /// </summary>
    /// <param name="userAreaCode">The <see cref="IUserAreaDefinition.UserAreaCode"/> identifier of the user area to restrict access to.</param>
    public AuthorizeUserAreaAttribute(string userAreaCode)
        : base()
    {
        AuthenticationSchemes = AuthenticationSchemeNames.UserArea(userAreaCode);
        Policy = AuthorizationPolicyNames.UserArea(userAreaCode);
    }
}
