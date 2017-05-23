using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return CustomEntitiesRouteLibrary.Js.JsFolderPath;
        }
    }
}