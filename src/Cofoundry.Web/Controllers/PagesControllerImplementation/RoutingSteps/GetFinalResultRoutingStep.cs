using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// In this last step we construct the view models and view result for the page. Some special page types
    /// have further actions applied (e.g. custom entity details pages).
    /// </summary>
    public class GetFinalResultRoutingStep : IGetFinalResultRoutingStep
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageViewModelMapper _pageViewModelMapper;
        private readonly IPageResponseDataCache _pageRenderDataCache;

        public GetFinalResultRoutingStep(
            IQueryExecutor queryExecutor,
            IPageViewModelMapper pageViewModelMapper,
            IPageResponseDataCache pageRenderDataCache
            )
        {
            _queryExecutor = queryExecutor;
            _pageViewModelMapper = pageViewModelMapper;
            _pageRenderDataCache = pageRenderDataCache;
        }

        public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            state.Result = await GetPageViewResult(controller, state);
        }

        private async Task<ActionResult> GetPageViewResult(Controller controller, PageActionRoutingState state)
        {
            IEditablePageViewModel vm;
            var pageRoutingInfo = state.PageRoutingInfo;

            // Some page types have thier own specific view models which custom data
            switch (pageRoutingInfo.PageRoute.PageType)
            {
                case PageType.NotFound:
                    controller.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    vm = _pageViewModelMapper.Map(state.PageData, state.SiteViewerMode);
                    break;
                case PageType.CustomEntityDetails:
                    var model = await GetCustomEntityModel(state);
                    vm = _pageViewModelMapper.MapCustomEntityModel(state.PageData.Template.CustomEntityModelType, state.PageData, model, state.SiteViewerMode);
                    break;
                //case PageType.Error:
                //    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //    vm = _pageViewModelMapper.Map(page, siteViewerMode);
                //    break;
                default:
                    vm = _pageViewModelMapper.Map(state.PageData, state.SiteViewerMode);
                    break;
            }


            // set cache
            SetCache(vm, state);

            var result = new ViewResult()
            {
                ViewData = new ViewDataDictionary(vm),
                ViewName = state.PageData.Template.FullPath
            };

            return result;
        }

        public void SetCache(IEditablePageViewModel vm, PageActionRoutingState state)
        {
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

            var pageResponseData = new PageResponseData();
            pageResponseData.Page = vm;
            pageResponseData.SiteViewerMode = siteViewerMode;
            pageResponseData.PageRoutingInfo = state.PageRoutingInfo;
            pageResponseData.HasDraftVersion = state.PageRoutingInfo.GetVersionRoute(state.InputParameters.IsEditingCustomEntity, WorkFlowStatusQuery.Draft, null) != null;
            pageResponseData.Version = state.PageRoutingInfo.GetVersionRoute(state.InputParameters.IsEditingCustomEntity, workFlowStatusQuery, state.InputParameters.VersionId);

            if (!string.IsNullOrEmpty(state.PageRoutingInfo.PageRoute.CustomEntityDefinitionCode))
            {
                pageResponseData.CustomEntityDefinition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(state.PageRoutingInfo.PageRoute.CustomEntityDefinitionCode);
            }

            if (state.InputParameters.IsEditingCustomEntity)
            {
                pageResponseData.PageVersion = pageVersions.GetVersionRouting(WorkFlowStatusQuery.Latest);
            }
            else
            {
                pageResponseData.PageVersion = pageVersions.GetVersionRouting(workFlowStatusQuery, state.InputParameters.VersionId);
            }

            _pageRenderDataCache.Set(pageResponseData);
        }

        private async Task<CustomEntityRenderDetails> GetCustomEntityModel(PageActionRoutingState state)
        {
            var query = new GetCustomEntityRenderDetailsByIdQuery();
            query.CustomEntityId = state.PageRoutingInfo.CustomEntityRoute.CustomEntityId;
            query.PageTemplateId = state.PageData.Template.PageTemplateId;

            // If we're editing the custom entity, we need to get the version we're editing, otherwise just get latest
            if (state.InputParameters.IsEditingCustomEntity)
            {
                query.WorkFlowStatus = state.SiteViewerMode.ToWorkFlowStatusQuery();
                query.CustomEntityVersionId = state.InputParameters.VersionId;
            }
            var model = await _queryExecutor.ExecuteAsync(query);
            return model;
        }
    }
}
