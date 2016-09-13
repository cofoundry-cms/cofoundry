using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A constraint used to work out if the requested page should be displayed inside 
    /// a 'site viewer' wrapper page. The site viewer is used for editing page content
    /// and will typically be used for any non-admin page when the user is logged in.
    /// </summary>
    public class SiteViewerRouteContraint : IRouteConstraint
    {
        /// <summary>
        /// I think this regex matches routes without file extensions, but i haven't
        /// checked it.
        /// </summary>
        const string PAGE_PATH_REGEX = @"^(?!(.*[\.].*))";

        /// <summary>
        /// At some point it would be better to allow this to be more pluggle and
        /// allow parts of the site to register that they shouldn't have a site viewer.
        /// An opt-in approach would be even better if possble to avoid running this for every request.
        /// </summary>
        private static string[] _routesToExclude = new string[] {
            Cofoundry.Web.Admin.RouteConstants.AdminUrlRoot,
            Cofoundry.Web.Admin.RouteConstants.ApiUrlRoot,
            "/api"
        };

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            bool requiresSiteViewer = false;
            var path = httpContext.Request.Url.AbsolutePath;

            if (Regex.IsMatch(path, PAGE_PATH_REGEX, RegexOptions.IgnoreCase)
                && !httpContext.Request.IsAjaxRequest()
                && !_routesToExclude.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase))
                && string.IsNullOrEmpty(httpContext.Request.QueryString["siteviewer"])
                && IsCofoundryUser()
                )
            {
                requiresSiteViewer = true;
            }

            return requiresSiteViewer;
        }

        private bool IsCofoundryUser()
        {
            // No constructor injection available
            var userContextService = IckyDependencyResolution.ResolveFromMvcContext<IUserContextService>();
            var userContext = userContextService.GetCurrentContext();

            return userContext.IsCofoundryUser();
        }
    }
}