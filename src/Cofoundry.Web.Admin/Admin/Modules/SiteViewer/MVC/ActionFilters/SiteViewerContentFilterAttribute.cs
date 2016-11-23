using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Adds in styles and js to run the site viewer if it is required.
    /// </summary>
    public class SiteViewerContentFilterAttribute : ActionFilterAttribute
    {
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

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var cofoundryUser = GetCofoundryUserIfCanShowSiteViewer(filterContext);

            if (cofoundryUser != null)
            {
                var responseDataCache = IckyDependencyResolution.ResolveFromMvcContext<IPageResponseDataCache>();
                var responseData = responseDataCache.Get();

                var response = filterContext.HttpContext.Response;
                var context = filterContext.Controller.ControllerContext;
                response.Filter = new SiteViewerContentStream(response.Filter, responseData, context);
            }
        }

        private IUserContext GetCofoundryUserIfCanShowSiteViewer(ResultExecutedContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            var response = httpContext.Response;

            var path = httpContext.Request.Url.AbsolutePath;

            var canShowSiteViewer =
                // Is authenticated in some way
                filterContext.HttpContext.User.Identity.IsAuthenticated
                // We have an exsting filter to override
                && response.Filter != null
                // Isn't a child action
                && !filterContext.IsChildAction
                // Isn't an ajax request
                && !httpContext.Request.IsAjaxRequest()
                // Is an html response
                && response.ContentType == "text/html"
                // Is a page and not a static resource
                && Regex.IsMatch(path, PAGE_PATH_REGEX, RegexOptions.IgnoreCase)
                // Isn't in the path blacklist
                && !_routesToExclude.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase));

            if (!canShowSiteViewer) return null;

            // Last check is if authenticated as a cofoundry user (this is most expensive test so do it last)
            return GetCofoundryUser();
        }

        private IUserContext GetCofoundryUser()
        {
            // No constructor injection available
            var userContextService = IckyDependencyResolution.ResolveFromMvcContext<IUserContextService>();
            var userContext = userContextService.GetCurrentContext();

            if (userContext.IsCofoundryUser())
            {
                return userContext;
            }

            return null;
        }
    }
}