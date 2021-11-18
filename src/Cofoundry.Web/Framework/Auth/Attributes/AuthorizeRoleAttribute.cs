using Cofoundry.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web
{
    /// <summary>
    /// Ensures that a class or method can only be accessed by users associated with 
    /// with a specific Cofoundry role. Only roles defined in code using <see cref="IRoleDefinition"/> 
    /// can be authorized with this attribute. Using this attribute will set the current user context 
    /// to the user area associated with the authorized role rather than the default.
    /// </summary>
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRoleAttribute"/> class.
        /// </summary>
        /// <param name="userAreaCode">The <see cref="IUserAreaDefinition.UserAreaCode"/> identifier of the user area that the role belongs to.</param>
        /// <param name="roleCode">The <see cref="IRoleDefinition.Rolecode"/> identifier of the code-defined role to restrict access to.</param>
        public AuthorizeRoleAttribute(string userAreaCode, string roleCode)
            : base()
        {
            AuthenticationSchemes = AuthenticationSchemeNames.UserArea(userAreaCode);
            Policy = AuthorizationPolicyNames.Role(userAreaCode, roleCode);
        }
    }
}