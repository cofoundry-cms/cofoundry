using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Core.Json;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// The default exclusion rules used by cofoundry to ensure the admin panel
    /// and any api routes don't have the visual editor rendered to them.
    /// </summary>
    public class DefaultVisualEditorRequestExclusionRule : IVisualEditorRequestExclusionRule
    {
        private static PathString[] _routesToExclude = new PathString[] {
                new PathString(RouteConstants.AdminUrlRoot),
                new PathString(RouteConstants.ApiUrlRoot),
                new PathString("/api")
            };

        public bool ShouldExclude(HttpRequest request)
        {
            return _routesToExclude.Any(r => request.Path.StartsWithSegments(r, StringComparison.OrdinalIgnoreCase));
        }
    }
}