using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    public class SiteViewerEmbeddedResourcetRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return SiteViewerRouteLibrary.Js.JsFolderPath;
            yield return SiteViewerRouteLibrary.StaticContent.EmbeddedResourceRegistrationPath;
        }
    }
}