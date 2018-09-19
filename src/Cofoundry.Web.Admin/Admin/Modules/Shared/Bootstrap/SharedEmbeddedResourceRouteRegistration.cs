using Cofoundry.Core.ResourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class SharedEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        const string ALTERNATIVE_ROUTE_PREFIX = "/Cofoundry";

        public SharedEmbeddedResourceRouteRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
        {
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