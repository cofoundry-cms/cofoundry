using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class SharedEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        const string ALTERNATIVE_ROUTE_PREFIX = "/Cofoundry";

        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly AdminSettings _adminSettings;

        public SharedEmbeddedResourceRouteRegistration(
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
                _adminRouteLibrary.Shared.GetStaticResourceFilePath(),
                _adminRouteLibrary.Shared.GetStaticResourceUrlPath()
                );
            yield return new EmbeddedResourcePath(
                assembly,
                _adminRouteLibrary.SharedAlternate.GetStaticResourceFilePath(),
                _adminRouteLibrary.SharedAlternate.GetStaticResourceUrlPath()
                );
        }
    }
}