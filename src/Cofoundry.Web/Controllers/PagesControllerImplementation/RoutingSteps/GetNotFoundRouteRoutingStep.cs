using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// If at this point we still do not have a valid PageRoutingInfo, we return a 404 result.
    /// This could itself be a Cofoundry Page so we search for that or fallback to a ageneric 404 result.
    /// </summary>
    public class GetNotFoundRouteRoutingStep : IGetNotFoundRouteRoutingStep
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly INotFoundViewHelper _notFoundViewHelper;
        private readonly IRedirectResponseHelper _redirectResponseHelper;
        private readonly ISiteViewerActionFactory _siteViewerActionFactory;

        public GetNotFoundRouteRoutingStep(
            IQueryExecutor queryExecutor,
            INotFoundViewHelper notFoundViewHelper,
            IRedirectResponseHelper redirectResponseHelper,
            ISiteViewerActionFactory siteViewerActionFactory
            )
        {
            _queryExecutor = queryExecutor;
            _notFoundViewHelper = notFoundViewHelper;
            _redirectResponseHelper = redirectResponseHelper;
            _siteViewerActionFactory = siteViewerActionFactory;
        }

        public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            // Find a 404 page if a version exists.
            if (state.PageRoutingInfo == null)
            {
                // First check for a rewrite rule and apply it
                state.Result = await GetRewriteResult(controller);
                if (state.Result != null) return;

                // else try and find a 404 page route
                state.PageRoutingInfo = await TryFindNotFoundPageRoute(state.InputParameters.Path, state.SiteViewerMode);

                // If we still can't find a 404, fall back to the generic 404 view
                if (state.PageRoutingInfo == null)
                {
                    state.Result = GetGenericPageNotFoundResult(controller, state);
                }
            }
        }

        private async Task<ActionResult> GetRewriteResult(Controller controller)
        {
            var query = new GetRewriteRuleByPathQuery() { Path = controller.Request.Path };
            var rewriteRule = await _queryExecutor.ExecuteAsync(query);
            if (rewriteRule != null)
            {
                string writeTo = rewriteRule.WriteTo;
                var response = new RedirectResult(rewriteRule.WriteTo, true);
                return _redirectResponseHelper.IncludeQueryParameters(response, "siteviewer");
            }

            return null;
        }

        /// <summary>
        /// Try and find a page route for a 404 page.
        /// </summary>
        private async Task<PageRoutingInfo> TryFindNotFoundPageRoute(string path, SiteViewerMode siteViewerMode)
        {
            var notFoundQuery = new GetNotFoundPageRouteByPathQuery()
            {
                Path = path,
                IncludeUnpublished = siteViewerMode != SiteViewerMode.Live
            };
            var pageRoute = await _queryExecutor.ExecuteAsync(notFoundQuery);

            if (pageRoute == null) return null;

            return new PageRoutingInfo()
            {
                PageRoute = pageRoute
            };
        }

        private ActionResult GetGenericPageNotFoundResult(Controller controller, PageActionRoutingState state)
        {
            ActionResult result = null;

            // NB: the page is not found, but this might fall through to a standard controller route.
            if (state.InputParameters.IsSiteViewerRequested && state.UserContext.IsCofoundryUser())
            {
                result = _siteViewerActionFactory.GetSiteViewerAction(controller, state);
            }

            if (result == null)
            {
                result = _notFoundViewHelper.GetView();
            }

            return result;
        }
    }
}
