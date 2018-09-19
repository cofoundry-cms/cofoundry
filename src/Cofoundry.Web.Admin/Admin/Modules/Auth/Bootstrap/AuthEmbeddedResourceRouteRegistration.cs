using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin
{
    public class AuthEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public AuthEmbeddedResourceRouteRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
        {
            var path = new EmbeddedResourcePath(
                GetType().Assembly,
                _adminRouteLibrary.Auth.GetStaticResourceFilePath(),
                _adminRouteLibrary.Auth.GetStaticResourceUrlPath()
                );

            yield return path;
        }
    }
}