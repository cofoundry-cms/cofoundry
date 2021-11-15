using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web
{
    /// <summary>
    /// Specifies that the class or method that this attribute is applied to can
    /// only be accessed by users associated with a specific user area. Using this
    /// attribute will set the current user context to the specified user area, rather 
    /// than the default.
    /// </summary>
    public class AuthorizeUserAreaAttribute : AuthorizeAttribute
    {
        public AuthorizeUserAreaAttribute(string userAreaCode)
            : base()
        {
            AuthenticationSchemes = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaCode);
        }
    }
}