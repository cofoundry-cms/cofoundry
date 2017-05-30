using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin
{
    public class SetupEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public SetupEmbeddedResourceRouteRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return _adminRouteLibrary.Setup.StaticResourcePrefix;
        }
    }
}