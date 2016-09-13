using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    public class SetupEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return SetupRouteLibrary.Js.JsFolderPath;
        }
    }
}