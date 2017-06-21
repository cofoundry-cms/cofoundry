using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public static class CofoundryAuthenticationConstants
    {
        /// <summary>
        /// Cookie authentication scheme used by Cofoundry to signin/signout i.e. HttpContext.Authentication.SignInAsync etc.
        /// </summary>
        public const string CookieAuthenticationScheme = "CofoundryCookieAuth";

        /// <summary>
        /// Formats the authorization policy name for the AuthorizeUserAreaAttribute.
        /// </summary>
        /// <param name="userAreaCode">The unique user area code to format a policy name for.</param>
        /// <returns>A unique namespaced policy name based on the user area code.</returns>
        public static string FormatPolicyName(string userAreaCode)
        {
            return "Cofoundry_UserArea_Policy_" + userAreaCode;
        }

        /// <summary>
        /// Formats the authorization scheme name for a specific user area. See
        /// https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme
        /// </summary>
        /// <param name="userAreaCode">The unique user area code to format a scheme name for.</param>
        /// <returns>A unique namespaced scheme name based on the user area code.</returns>
        public static string FormatAuthenticationScheme(string userAreaCode)
        {
            return "Cofoundry_UserArea_Scheme_" + userAreaCode;
        }
    }
}