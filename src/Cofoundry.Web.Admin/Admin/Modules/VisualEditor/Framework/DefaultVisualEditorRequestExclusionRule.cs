using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly AdminSettings _adminSettings;

        public DefaultVisualEditorRequestExclusionRule(
            AdminSettings adminSettings
            )
        {
            _adminSettings = adminSettings;
        }

        public bool ShouldExclude(HttpRequest request)
        {
            return GetRoutes().Any(r => request.Path.StartsWithSegments(r, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<PathString> GetRoutes()
        {
            yield return '/' + _adminSettings.DirectoryName;
            yield return "/api";
        }
    }
}