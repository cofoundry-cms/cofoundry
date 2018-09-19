using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class UsersEmbeddedRouteRegistrations : IEmbeddedResourceRouteRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly AdminSettings _adminSettings;

        public UsersEmbeddedRouteRegistrations(
            IAdminRouteLibrary adminRouteLibrary,
            AdminSettings adminSettings
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
            _adminSettings = adminSettings;
        }

        public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
        {
            if (_adminSettings.Disabled) yield break;

            var assembly = GetType().Assembly;

            yield return new EmbeddedResourcePath(
                assembly,
                _adminRouteLibrary.Users.GetStaticResourceFilePath(),
                _adminRouteLibrary.Users.GetStaticResourceUrlPath()
                );
        }
    }
}