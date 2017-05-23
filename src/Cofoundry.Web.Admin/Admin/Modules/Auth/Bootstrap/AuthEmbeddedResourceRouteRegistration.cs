using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin
{
    public class AuthEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return AuthRouteLibrary.StaticContent.EmbeddedResourceRegistrationPath;
            yield return AuthRouteLibrary.Js.MvcViewFolderPath;
        }
    }
}