using Cofoundry.Domain;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Cofoundry.Web
{
    /// <inheritdoc/>
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
