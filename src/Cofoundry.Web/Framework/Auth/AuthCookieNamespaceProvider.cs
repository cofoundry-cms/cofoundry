using Cofoundry.Domain;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to get a string that is used to make the auth cookies unique. The 
    /// user area code will be appended to this to make the cookiename, e.g.
    /// "MyAppAuth_COF". By default the cookie namespace is created
    /// using characters from the entry assembly name of your applicaiton, but
    /// you can override this behaviour using the Cofoundry:Auth:CookieNamespace
    /// config setting.
    /// </summary>
    public class AuthCookieNamespaceProvider : IAuthCookieNamespaceProvider
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly AuthenticationSettings _authenticationSettings;

        public AuthCookieNamespaceProvider(
            IHostEnvironment hostingEnvironment,
            AuthenticationSettings authenticationSettings
            )
        {
            _hostingEnvironment = hostingEnvironment;
            _authenticationSettings = authenticationSettings;
        }

        /// <summary>
        /// Gets a string that can be used to uniquely namespace the auth cookies
        /// for this application.
        /// </summary>
        public string GetNamespace()
        {
            if (!string.IsNullOrWhiteSpace(_authenticationSettings.CookieNamespace))
            {
                return _authenticationSettings.CookieNamespace;
            }

            // Try and build a short and somewhat unique name using the 
            // application name, which should suffice for most scenarios. 
            var appName = _hostingEnvironment.ApplicationName;

            var reasonablyUniqueName = appName
                .Take(3)
                .Union(appName.Reverse())
                .Take(6);

            return "CFA_" + string.Concat(reasonablyUniqueName);
        }
    }
}
