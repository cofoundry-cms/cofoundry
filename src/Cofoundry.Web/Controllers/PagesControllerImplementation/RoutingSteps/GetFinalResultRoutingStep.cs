using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// In this last step we construct the view models and view result for the page. Some special page types
    /// have further actions applied (e.g. custom entity details pages).
    /// </summary>
    public class GetFinalResultRoutingStep : IGetFinalResultRoutingStep
    {
        private static readonly MethodInfo _methodInfo_GenericBuildCustomEntityModelAsync = typeof(GetFinalResultRoutingStep).GetMethod(nameof(GenericBuildCustomEntityModelAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageViewModelBuilder _pageViewModelBuilder;
        private readonly IPageResponseDataCache _pageRenderDataCache;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetFinalResultRoutingStep(
            IQueryExecutor queryExecutor,
            IPageViewModelBuilder pageViewModelBuilder,
            IPageResponseDataCache pageRenderDataCache,
            IPermissionValidationService permissionValidationService
            )
        {
            _queryExecutor = queryExecutor;
            _pageViewModelBuilder = pageViewModelBuilder;
            _pageRenderDataCache = pageRenderDataCache;
            _permissionValidationService = permissionValidationService;
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
                    // Not sure why we're not using a NotFoundViewModel here, but this is old
                    // and untested functionality. Content managable not found pages will need to be looked at at a later date
                    var notFoundPageParams = new PageViewModelBuilderParameters(state.PageData, state.VisualEditorState.VisualEditorMode);
                    vm = await _pageViewModelBuilder.BuildPageViewModelAsync(notFoundPageParams);
                    break;
                case PageType.CustomEntityDetails:
                    var model = await GetCustomEntityModel(state);
                    var customEntityParams = new CustomEntityPageViewModelBuilderParameters(state.PageData, state.VisualEditorState.VisualEditorMode, model);

                    vm = await BuildCustomEntityViewModelAsync(state.PageData.Template.CustomEntityModelType, customEntityParams);
                    break;
                //case PageType.Error:
                //    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //    vm = _pageViewModelMapper.MapPage(page, siteViewerMode);
                //    break;
                default:
                    var pageParams = new PageViewModelBuilderParameters(state.PageData, state.VisualEditorState.VisualEditorMode);
                    vm = await _pageViewModelBuilder.BuildPageViewModelAsync(pageParams);
                    break;
            }


            // set cache
            await SetCacheAsync(vm, state);

            var result = controller.View(state.PageData.Template.FullPath, vm);
            return result;
        }

        public async Task SetCacheAsync(IEditablePageViewModel vm, PageActionRoutingState state)
        {
            var visualEditorMode = state.VisualEditorState.VisualEditorMode;
            var publishStatusQuery = visualEditorMode.ToPublishStatusQuery();
            var pageVersions = state.PageRoutingInfo.PageRoute.Versions;

            // Force a viewer mode
            if (visualEditorMode == VisualEditorMode.Any)
            {
                var version = state.PageRoutingInfo.GetVersionRoute(
                    state.InputParameters.IsEditingCustomEntity,
                    publishStatusQuery,
                    state.InputParameters.VersionId);

                switch (version.WorkFlowStatus)
                {
                    case WorkFlowStatus.Draft:
                        visualEditorMode = VisualEditorMode.Preview;
                        break;
                    case WorkFlowStatus.Published:
                        visualEditorMode = VisualEditorMode.Live;
                        break;
                    default:
                        throw new InvalidOperationException("WorkFlowStatus." + version.WorkFlowStatus + " is not valid for VisualEditorMode.Any");
                }
            }

            var pageResponseData = new PageResponseData();
            pageResponseData.Page = vm;
            pageResponseData.VisualEditorMode = visualEditorMode;
            pageResponseData.PageRoutingInfo = state.PageRoutingInfo;
            pageResponseData.HasDraftVersion = state.PageRoutingInfo.GetVersionRoute(state.InputParameters.IsEditingCustomEntity, PublishStatusQuery.Draft, null) != null;
            pageResponseData.Version = state.PageRoutingInfo.GetVersionRoute(state.InputParameters.IsEditingCustomEntity, publishStatusQuery, state.InputParameters.VersionId);
            pageResponseData.CofoundryAdminUserContext = state.CofoundryAdminUserContext;

            var customEntityDefinitionCode = state.PageRoutingInfo.PageRoute.CustomEntityDefinitionCode;
            if (!string.IsNullOrEmpty(customEntityDefinitionCode))
            {
                var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(customEntityDefinitionCode);
                pageResponseData.CustomEntityDefinition = await _queryExecutor.ExecuteAsync(definitionQuery);
            }

            if (state.InputParameters.IsEditingCustomEntity)
            {
                pageResponseData.PageVersion = pageVersions.GetVersionRouting(PublishStatusQuery.Latest);
            }
            else
            {
                pageResponseData.PageVersion = pageVersions.GetVersionRouting(publishStatusQuery, state.InputParameters.VersionId);
            }

            _pageRenderDataCache.Set(pageResponseData);
        }

        private async Task<CustomEntityRenderDetails> GetCustomEntityModel(PageActionRoutingState state)
        {
            var query = new GetCustomEntityRenderDetailsByIdQuery();
            query.CustomEntityId = state.PageRoutingInfo.CustomEntityRoute.CustomEntityId;
            query.PageId = state.PageData.PageId;

            // If we're editing the custom entity, we need to get the version we're editing, otherwise just get latest
            if (state.InputParameters.IsEditingCustomEntity)
            {
                if (state.InputParameters.VersionId.HasValue)
                {
                    query.CustomEntityVersionId = state.InputParameters.VersionId;
                    query.PublishStatus = PublishStatusQuery.SpecificVersion;
                }
                else
                {
                    query.PublishStatus = state.VisualEditorState.GetPublishStatusQuery();
                }
            }
            else if (state.IsCofoundryAdminUser)
            {
                query.PublishStatus = PublishStatusQuery.Latest;
            }

            var model = await _queryExecutor.ExecuteAsync(query);
            return model;
        }

        private async Task<IEditablePageViewModel> BuildCustomEntityViewModelAsync(
            Type displayModelType,
            CustomEntityPageViewModelBuilderParameters mappingParameters
            )
        {
            var task = (Task<IEditablePageViewModel>)_methodInfo_GenericBuildCustomEntityModelAsync
                .MakeGenericMethod(displayModelType)
                .Invoke(this, new object[] { mappingParameters });

            return await task;
        }

        private async Task<IEditablePageViewModel> GenericBuildCustomEntityModelAsync<TDisplayModel>(
            CustomEntityPageViewModelBuilderParameters mappingParameters
            ) where TDisplayModel : ICustomEntityPageDisplayModel
        {
            var result = await _pageViewModelBuilder.BuildCustomEntityPageViewModelAsync<TDisplayModel>(mappingParameters);
            return result;
        }
    }
}
