using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin
{
    public class UsersEmbeddedRouteRegistrations : IEmbeddedResourceRouteRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public UsersEmbeddedRouteRegistrations(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return _adminRouteLibrary.Users.StaticResourcePrefix;
        }
    }
}