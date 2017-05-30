using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public VisualEditorEmbeddedResourceRouteRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return _adminRouteLibrary.VisualEditor.StaticResourcePrefix;
        }
    }
}