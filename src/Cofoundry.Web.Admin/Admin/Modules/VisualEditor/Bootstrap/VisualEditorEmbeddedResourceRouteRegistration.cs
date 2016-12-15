using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorEmbeddedResourcetRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return VisualEditorRouteLibrary.Js.JsFolderPath;
            yield return VisualEditorRouteLibrary.StaticContent.EmbeddedResourceRegistrationPath;
        }
    }
}