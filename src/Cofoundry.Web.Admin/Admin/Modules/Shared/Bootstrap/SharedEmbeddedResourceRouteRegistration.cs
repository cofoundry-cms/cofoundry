using Cofoundry.Core.ResourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return _adminRouteLibrary.Shared.StaticResourcePrefix;
            yield return ALTERNATIVE_ROUTE_PREFIX + _adminRouteLibrary.Shared.StaticResourcePrefix;
        }
    }
}