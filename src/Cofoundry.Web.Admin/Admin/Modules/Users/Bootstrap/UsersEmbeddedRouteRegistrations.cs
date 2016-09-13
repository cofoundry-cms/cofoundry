using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    public class UsersResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return UsersRouteLibrary.Js.JsFolderPath;
        }
    }
}