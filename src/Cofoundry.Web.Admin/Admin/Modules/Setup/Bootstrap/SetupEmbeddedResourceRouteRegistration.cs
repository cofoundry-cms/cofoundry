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

        public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
        {
            var path = new EmbeddedResourcePath(
                GetType().Assembly,
                _adminRouteLibrary.Setup.GetStaticResourceFilePath(),
                _adminRouteLibrary.Setup.GetStaticResourceUrlPath()
                );

            yield return path;
        }
    }
}