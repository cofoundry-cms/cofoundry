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

        public GetFinalResultRoutingStep(
            IQueryExecutor queryExecutor,
            IPageViewModelMapper pageViewModelMapper
            )
        {
            _queryExecutor = queryExecutor;
            _pageViewModelMapper = pageViewModelMapper;
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

            var result = new ViewResult()
            {
                ViewData = new ViewDataDictionary(vm),
                ViewName = state.PageData.Template.FullPath
            };

            return result;
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
