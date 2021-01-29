using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public static class CofoundryAuthenticationConstants
    {
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