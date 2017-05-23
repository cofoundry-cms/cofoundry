using Cofoundry.Core.ResourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SharedEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        const string ALTERNATIVE_ROUTE_PREFIX = "/Cofoundry";

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return SharedRouteLibrary.StaticContent.EmbeddedResourceRegistrationPath;
            yield return SharedRouteLibrary.Js.JsFolderPath;
            yield return ALTERNATIVE_ROUTE_PREFIX + SharedRouteLibrary.StaticContent.EmbeddedResourceRegistrationPath;
            yield return ALTERNATIVE_ROUTE_PREFIX + SharedRouteLibrary.Js.JsFolderPath;
        }
    }
}