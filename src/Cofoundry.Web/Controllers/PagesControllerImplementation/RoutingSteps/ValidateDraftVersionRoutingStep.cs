using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks to see if there is a valid page version for the request, if not
    /// one is created.
    /// </summary>
    public class ValidateDraftVersionRoutingStep : IValidateDraftVersionRoutingStep
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public ValidateDraftVersionRoutingStep(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            var pageRoutingInfo = state.PageRoutingInfo;

            if (pageRoutingInfo == null) return;

            // If there's no draft version and we're in edit mode for 'Pages' then
            // create one and re-run the query
            var workflowStatus = state.SiteViewerMode.ToWorkFlowStatusQuery();
            if (!state.InputParameters.IsEditingCustomEntity
                && workflowStatus != WorkFlowStatusQuery.Published
                && workflowStatus != WorkFlowStatusQuery.SpecificVersion
                && state.UserContext.IsCofoundryUser())
            {
                var versionRouting = pageRoutingInfo.PageRoute.Versions.GetVersionRouting(workflowStatus);
                if (versionRouting == null)
                {
                    await _commandExecutor.ExecuteAsync(new AddPageDraftVersionCommand() { PageId = pageRoutingInfo.PageRoute.PageId });
                    var pageQuery = GetPageRoutingInfoQuery(state);
                    pageRoutingInfo = await _queryExecutor.ExecuteAsync(pageQuery);
                }
            }

            if (pageRoutingInfo.CustomEntityRoute != null
                && pageRoutingInfo.PageRoute.PageType == PageType.CustomEntityDetails)
            {
                var correctUrl = pageRoutingInfo.CustomEntityRouteRule.MakeUrl(pageRoutingInfo.PageRoute, pageRoutingInfo.CustomEntityRoute);
                if (!state.InputParameters.Path.Equals(correctUrl.TrimStart('/'), StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("Incorrect Custom Entity Url detected, redirecting from " + state.InputParameters.Path + " to " + correctUrl);
                    state.Result = new RedirectResult(correctUrl, true);
                }
                else if (state.InputParameters.IsEditingCustomEntity
                    && workflowStatus != WorkFlowStatusQuery.Published
                    && workflowStatus != WorkFlowStatusQuery.SpecificVersion
                    && state.UserContext.IsCofoundryUser())
                {
                    var versionRouting = pageRoutingInfo.CustomEntityRoute.Versions.GetVersionRouting(workflowStatus);
                    if (versionRouting == null)
                    {
                        // if no draft version for route, add one.
                        await _commandExecutor.ExecuteAsync(new AddCustomEntityDraftVersionCommand() { CustomEntityId = pageRoutingInfo.CustomEntityRoute.CustomEntityId });
                        var pageQuery = GetPageRoutingInfoQuery(state);
                        pageRoutingInfo = await _queryExecutor.ExecuteAsync(pageQuery);
                    }
                }
            }

            state.PageRoutingInfo = pageRoutingInfo;
        }

        private GetPageRoutingInfoByPathQuery GetPageRoutingInfoQuery(PageActionRoutingState state)
        {
            var pageQuery = new GetPageRoutingInfoByPathQuery()
            {
                Path = state.InputParameters.Path,
                IncludeUnpublished = state.SiteViewerMode != SiteViewerMode.Live
            };

            if (state.Locale != null)
            {
                pageQuery.LocaleId = state.Locale.LocaleId;
            }

            return pageQuery;
        }
    }
}
