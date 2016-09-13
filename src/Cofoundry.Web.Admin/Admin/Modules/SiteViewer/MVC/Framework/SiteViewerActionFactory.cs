using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web.Admin
{
    public class SiteViewerActionFactory : ISiteViewerActionFactory
    {
        private readonly IQueryExecutor _queryExecutor;

        public SiteViewerActionFactory(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public ActionResult GetSiteViewerAction(Controller controller, PageActionRoutingState state = null)
        {
            if (state == null || state.PageRoutingInfo == null) return GetStaticPageSiteViewer();
            Condition.Requires(state.InputParameters, "state.InputParameters");

            var siteViewerMode = state.SiteViewerMode;
            var workFlowStatusQuery = state.SiteViewerMode.ToWorkFlowStatusQuery();
            var pageVersions = state.PageRoutingInfo.PageRoute.Versions;

            // Force a viewer mode
            if (siteViewerMode == SiteViewerMode.Any)
            {
                var version = state.PageRoutingInfo.GetVersionRoute(
                    state.InputParameters.IsEditingCustomEntity,
                    state.SiteViewerMode.ToWorkFlowStatusQuery(),
                    state.InputParameters.VersionId);

                switch (version.WorkFlowStatus)
                {
                    case WorkFlowStatus.Draft:
                        siteViewerMode = SiteViewerMode.Draft;
                        break;
                    case WorkFlowStatus.Published:
                        siteViewerMode = SiteViewerMode.Live;
                        break;
                    default:
                        throw new ApplicationException("WorkFlowStatus." + version.WorkFlowStatus + " is not valid for SiteViewerMode.Any");
                }
            }

            var vm = new SiteViewerPageViewModel
            {
                SiteViewerMode = siteViewerMode,
                PageRoutingInfo = state.PageRoutingInfo,
                HasDraftVersion = state.PageRoutingInfo.GetVersionRoute(state.InputParameters.IsEditingCustomEntity, WorkFlowStatusQuery.Draft, null) != null,
                Version = state.PageRoutingInfo.GetVersionRoute(state.InputParameters.IsEditingCustomEntity, workFlowStatusQuery, state.InputParameters.VersionId)
            };

            if (!string.IsNullOrEmpty(state.PageRoutingInfo.PageRoute.CustomEntityDefinitionCode))
            {
                vm.CustomEntityDefinition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(state.PageRoutingInfo.PageRoute.CustomEntityDefinitionCode);
            }

            if (state.InputParameters.IsEditingCustomEntity)
            {
                vm.PageVersion = pageVersions.GetVersionRouting(WorkFlowStatusQuery.Latest);
            }
            else
            {
                vm.PageVersion = pageVersions.GetVersionRouting(workFlowStatusQuery, state.InputParameters.VersionId);
            }

            var viewResult = new ViewResult()
            {
                ViewData = new ViewDataDictionary(vm),
                ViewName = SiteViewerRouteLibrary.SiteViewerViewPath()
            };

            return viewResult;
        }

        private static ActionResult GetStaticPageSiteViewer()
        {
            var vm = new SiteViewerPageViewModel
            {
                IsStaticPage = true,
                SiteViewerMode = SiteViewerMode.Live
            };

            return new ViewResult()
            {
                ViewData = new ViewDataDictionary(vm),
                ViewName = SiteViewerRouteLibrary.SiteViewerViewPath()
            };
        }
    }
}