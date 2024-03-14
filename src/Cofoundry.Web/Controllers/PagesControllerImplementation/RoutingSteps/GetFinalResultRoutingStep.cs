using Cofoundry.Core.Reflection.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;

namespace Cofoundry.Web;

/// <summary>
/// In this last step we construct the view models and view result for the page. Some special page types
/// have further actions applied (e.g. custom entity details pages).
/// </summary>
public class GetFinalResultRoutingStep : IGetFinalResultRoutingStep
{
    private static readonly MethodInfo _methodInfo_GenericBuildCustomEntityModelAsync = MethodReferenceHelper.GetPrivateInstanceMethod<GetFinalResultRoutingStep>(nameof(GenericBuildCustomEntityModelAsync));

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
        ArgumentNullException.ThrowIfNull(state.PageRoutingInfo);
        ArgumentNullException.ThrowIfNull(state.PageData);
        ArgumentNullException.ThrowIfNull(state.VisualEditorState);

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
                EntityInvalidOperationException.ThrowIfNull(state.PageData.Template, state.PageData.Template.CustomEntityModelType);
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
        EntityInvalidOperationException.ThrowIfNull(state, state.PageRoutingInfo);
        EntityInvalidOperationException.ThrowIfNull(state, state.VisualEditorState);

        var visualEditorMode = state.VisualEditorState.VisualEditorMode;
        var publishStatusQuery = visualEditorMode.ToPublishStatusQuery();
        var pageVersions = state.PageRoutingInfo.PageRoute.Versions;

        var entityVersion = state.PageRoutingInfo.GetVersionRoute(
            state.InputParameters.IsEditingCustomEntity,
            publishStatusQuery,
            state.InputParameters.VersionId
            );

        if (entityVersion == null)
        {
            var entityDescription = state.InputParameters.IsEditingCustomEntity ? "custom entity" : "page";
            throw new InvalidOperationException($"Could not find a valid {entityDescription} version for page '{state.PageRoutingInfo.PageRoute.PageId}', {nameof(PublishStatusQuery)}.'{publishStatusQuery}, VersionId '{state.InputParameters.VersionId}'");
        }

        var pageVersion = state.InputParameters.IsEditingCustomEntity
            ? pageVersions.GetVersionRouting(PublishStatusQuery.Latest)
            : pageVersions.GetVersionRouting(publishStatusQuery, state.InputParameters.VersionId);

        if (pageVersion == null)
        {
            throw new InvalidOperationException($"Could not find a valid page version for page '{state.PageRoutingInfo.PageRoute.PageId}', {nameof(PublishStatusQuery)}.'{publishStatusQuery}, VersionId '{state.InputParameters.VersionId}'");
        }

        // Force a viewer mode
        if (visualEditorMode == VisualEditorMode.Any)
        {
            switch (entityVersion.WorkFlowStatus)
            {
                case WorkFlowStatus.Draft:
                    visualEditorMode = VisualEditorMode.Preview;
                    break;
                case WorkFlowStatus.Published:
                    visualEditorMode = VisualEditorMode.Live;
                    break;
                default:
                    throw new InvalidOperationException($"WorkFlowStatus.{entityVersion.WorkFlowStatus} is not valid for VisualEditorMode.Any");
            }
        }

        var pageResponseData = new PageResponseData
        {
            Page = vm,
            VisualEditorMode = visualEditorMode,
            PageRoutingInfo = state.PageRoutingInfo,
            HasDraftVersion = state.PageRoutingInfo.GetVersionRoute(state.InputParameters.IsEditingCustomEntity, PublishStatusQuery.Draft, null) != null,
            Version = entityVersion,
            CofoundryAdminUserContext = state.CofoundryAdminUserContext,
            PageVersion = pageVersion
        };

        var customEntityDefinitionCode = state.PageRoutingInfo.PageRoute.CustomEntityDefinitionCode;
        if (!string.IsNullOrEmpty(customEntityDefinitionCode))
        {
            var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(customEntityDefinitionCode);
            pageResponseData.CustomEntityDefinition = await _queryExecutor.ExecuteAsync(definitionQuery);
        }

        _pageRenderDataCache.Set(pageResponseData);
    }

    private async Task<CustomEntityRenderDetails> GetCustomEntityModel(PageActionRoutingState state)
    {
        ArgumentNullException.ThrowIfNull(state.PageRoutingInfo);
        ArgumentNullException.ThrowIfNull(state.VisualEditorState);
        EntityInvalidOperationException.ThrowIfNull(state, state.PageRoutingInfo.CustomEntityRoute);
        EntityInvalidOperationException.ThrowIfNull(state, state.PageData);

        var query = new GetCustomEntityRenderDetailsByIdQuery
        {
            CustomEntityId = state.PageRoutingInfo.CustomEntityRoute.CustomEntityId,
            PageId = state.PageData.PageId
        };

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

        var customEntity = await _queryExecutor.ExecuteAsync(query);
        EntityNotFoundException.ThrowIfNull(customEntity);

        return customEntity;
    }

    private async Task<IEditablePageViewModel> BuildCustomEntityViewModelAsync(
        Type displayModelType,
        CustomEntityPageViewModelBuilderParameters mappingParameters
        )
    {
        var task = _methodInfo_GenericBuildCustomEntityModelAsync
            .MakeGenericMethod(displayModelType)
            .Invoke(this, new object[] { mappingParameters }) as Task<IEditablePageViewModel>;

        if (task == null)
        {
            throw new InvalidOperationException($"{nameof(GenericBuildCustomEntityModelAsync)} did not return the expected type.");
        }

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
