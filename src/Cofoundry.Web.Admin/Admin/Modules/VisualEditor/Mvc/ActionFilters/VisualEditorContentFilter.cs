using Cofoundry.Core;
using Cofoundry.Core.Json;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Adds in styles and js to run the site viewer if it is required.
    /// </summary>
    public class VisualEditorContentFilter : IAsyncResultFilter
    {
        const string PAGE_PATH_REGEX = @"^(?!(.*[\.].*))";

        /// <summary>
        /// At some point it would be better to allow this to be more pluggable and
        /// allow parts of the site to register that they shouldn't have a site viewer.
        /// An opt-in approach would be even better if possble to avoid running this for every request.
        /// </summary>
        private static PathString[] _routesToExclude = new PathString[] {
                new PathString(Cofoundry.Web.Admin.RouteConstants.AdminUrlRoot),
                new PathString(Cofoundry.Web.Admin.RouteConstants.ApiUrlRoot),
                new PathString("/api")
            };

        private readonly IVisualEditorActionResultFactory _visualEditorActionResultFactory;
        private readonly IUserContextService _userContextService;

        public VisualEditorContentFilter(
            IVisualEditorActionResultFactory visualEditorActionResultFactory,
            IUserContextService userContextService
            )
        {
            _visualEditorActionResultFactory = visualEditorActionResultFactory;
            _userContextService = userContextService;
        }


        public async Task OnResultExecutionAsync(
            ResultExecutingContext context, 
            ResultExecutionDelegate next
            )
        {

            var cofoundryUser = await GetCofoundryUserIfCanShowVisualEditorAsync(context);

            if (cofoundryUser != null)
            {
                context.Result = _visualEditorActionResultFactory.Create(context.Result);
            }

            await next();
        }

        private Task<IUserContext> GetCofoundryUserIfCanShowVisualEditorAsync(ResultExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            var path = httpContext.Request.Path;

            var canShowSiteViewer =
                // Is authenticated in some way
                httpContext.User.Identities.Any()
                // We have an exsting filter to override
                && filterContext.Result != null
                // Is a get request
                && httpContext.Request.Method == "GET"
                // Isn't an ajax request
                && httpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest"
                // Is a page and not a static resource
                && Regex.IsMatch(path, PAGE_PATH_REGEX, RegexOptions.IgnoreCase)
                // Isn't in the path blacklist
                && !_routesToExclude.Any(r => path.StartsWithSegments(r, StringComparison.OrdinalIgnoreCase));

            if (!canShowSiteViewer) return Task.FromResult<IUserContext>(null);

            // Last check is if authenticated as a cofoundry user (this is most expensive test so do it last)
            return GetCofoundryUserAsync();
        }

        private async Task<IUserContext> GetCofoundryUserAsync()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();

            if (userContext.IsCofoundryUser())
            {
                return userContext;
            }

            return null;
        }
    }
}