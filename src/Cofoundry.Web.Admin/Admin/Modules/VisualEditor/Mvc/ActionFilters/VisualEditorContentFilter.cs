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
        
        private readonly IVisualEditorActionResultFactory _visualEditorActionResultFactory;
        private readonly IUserContextService _userContextService;
        private readonly IEnumerable<IVisualEditorRequestExclusionRule> _visualEditorRouteExclusionRules;

        public VisualEditorContentFilter(
            IVisualEditorActionResultFactory visualEditorActionResultFactory,
            IUserContextService userContextService,
            IEnumerable<IVisualEditorRequestExclusionRule> visualEditorRouteExclusionRules
            )
        {
            _visualEditorActionResultFactory = visualEditorActionResultFactory;
            _userContextService = userContextService;
            _visualEditorRouteExclusionRules = visualEditorRouteExclusionRules;
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
                // We have an exsting filter to override
                filterContext.Result != null
                // Valid Result Type
                && IsValidActionType(filterContext.Result)
                // Isn't an ajax request
                && httpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest"
                // Is a page and not a static resource
                && Regex.IsMatch(path, PAGE_PATH_REGEX, RegexOptions.IgnoreCase)
                // Isn't in the path blacklist
                && !_visualEditorRouteExclusionRules.Any(r => r.ShouldExclude(httpContext.Request));

            if (!canShowSiteViewer) return Task.FromResult<IUserContext>(null);

            // Last check is if authenticated as a cofoundry user (this is most expensive test so do it last)
            return GetCofoundryUserAsync();
        }

        private bool IsValidActionType(IActionResult actionResult)
        {
            if (actionResult is FileResult) return false;
            if (actionResult is StatusCodeResult) return false;

            return true;
        }

        private async Task<IUserContext> GetCofoundryUserAsync()
        {
            // The ambient auth scheme may not be for CofoundryAdmin so make sure we get 
            var userContext = await _userContextService.GetCurrentContextByUserAreaAsync(CofoundryAdminUserArea.AreaCode);

            if (userContext.IsCofoundryUser())
            {
                return userContext;
            }

            return null;
        }
    }
}