using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    public class AuthEmbeddedResourcetRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return AuthRouteLibrary.StaticContent.EmbeddedResourceRegistrationPath;
            yield return AuthRouteLibrary.Js.MvcViewFolderPath;
        }
    }
}