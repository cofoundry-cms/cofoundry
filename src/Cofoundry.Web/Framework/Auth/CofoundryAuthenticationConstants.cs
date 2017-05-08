using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    public static class CofoundryAuthenticationConstants
    {
        /// <summary>
        /// Cookie authentication scheme used by Cofoundry to signin/signout i.e. HttpContext.Authentication.SignInAsync etc.
        /// </summary>
        public const string CookieAuthenticationScheme = "CofoundryCookieAuth";
    }
}