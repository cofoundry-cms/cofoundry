using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return VisualEditorRouteLibrary.Js.JsFolderPath;
            yield return VisualEditorRouteLibrary.StaticContent.EmbeddedResourceRegistrationPath;
        }
    }
}