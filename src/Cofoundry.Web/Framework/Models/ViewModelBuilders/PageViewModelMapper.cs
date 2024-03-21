using System.Net;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IPageViewModelMapper"/>. You can 
/// override this implementation to customize the mapping behaviour.
/// </summary>
public class PageViewModelMapper : IPageViewModelMapper
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICustomEntityDisplayModelMapper _customEntityDisplayModelMapper;

    public PageViewModelMapper(
        IQueryExecutor queryExecutor,
        ICustomEntityDisplayModelMapper customEntityDisplayModelMapper
        )
    {
        _queryExecutor = queryExecutor;
        _customEntityDisplayModelMapper = customEntityDisplayModelMapper;
    }

    /// <inheritdoc/>
    public virtual Task MapPageViewModelAsync(
        IPageViewModel viewModel,
        PageViewModelBuilderParameters mappingParameters
        )
    {
        return MapAsync(viewModel, mappingParameters);
    }

    /// <inheritdoc/>
    public virtual async Task MapCustomEntityViewModelAsync<TDisplayModel>(
        ICustomEntityPageViewModel<TDisplayModel> viewModel,
        CustomEntityPageViewModelBuilderParameters mappingParameters
        ) where TDisplayModel : ICustomEntityPageDisplayModel
    {
        await MapAsync(viewModel, mappingParameters);

        ArgumentNullException.ThrowIfNull(mappingParameters.CustomEntityModel);

        var customEntityRenderDetails = mappingParameters.CustomEntityModel;
        var publishStatusQuery = mappingParameters.VisualEditorMode.ToPublishStatusQuery();
        var model = await _customEntityDisplayModelMapper.MapDisplayModelAsync<TDisplayModel>(customEntityRenderDetails, publishStatusQuery);

        var customModel = new CustomEntityRenderDetailsViewModel<TDisplayModel>
        {
            CustomEntityId = customEntityRenderDetails.CustomEntityId,
            CustomEntityVersionId = customEntityRenderDetails.CustomEntityVersionId,
            Locale = customEntityRenderDetails.Locale,
            Regions = customEntityRenderDetails.Regions,
            Title = customEntityRenderDetails.Title,
            UrlSlug = customEntityRenderDetails.UrlSlug,
            WorkFlowStatus = customEntityRenderDetails.WorkFlowStatus,
            PublishDate = customEntityRenderDetails.PublishDate,
            PublishStatus = customEntityRenderDetails.PublishStatus,
            CreateDate = customEntityRenderDetails.CreateDate,
            PageUrls = customEntityRenderDetails.PageUrls,
            Model = model
        };

        viewModel.CustomEntity = customModel;
    }

    /// <inheritdoc/>
    public virtual Task MapNotFoundPageViewModelAsync(
        INotFoundPageViewModel viewModel,
        NotFoundPageViewModelBuilderParameters mappingParameters
        )
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(mappingParameters);

        viewModel.PageTitle = "Not found";
        viewModel.MetaDescription = "Sorry, that page could not be found";
        viewModel.Path = mappingParameters.Path;
        viewModel.PathBase = mappingParameters.PathBase;
        viewModel.QueryString = mappingParameters.QueryString;
        viewModel.StatusCode = 404;
        viewModel.StatusCodeDescription = "Not Found";

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task MapErrorPageViewModelAsync(
        IErrorPageViewModel viewModel,
        ErrorPageViewModelBuilderParameters mappingParameters
        )
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(mappingParameters);
        ArgumentOutOfRangeException.ThrowIfLessThan(mappingParameters.StatusCode, 100);

        viewModel.StatusCode = mappingParameters.StatusCode;
        viewModel.StatusCodeDescription = GetStatusCodeDescription(viewModel.StatusCode);
        viewModel.PageTitle = "Error: " + viewModel.StatusCodeDescription;
        viewModel.MetaDescription = "Sorry, an error has occurred";
        viewModel.Path = mappingParameters.Path;
        viewModel.PathBase = mappingParameters.PathBase;
        viewModel.QueryString = mappingParameters.QueryString;

        return Task.CompletedTask;
    }

    private static string GetStatusCodeDescription(int statusCode)
    {
        if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
        {
            return "Unknown";
        }

        var statusCodeEnum = (HttpStatusCode)statusCode;
        var result = TextFormatter.PascalCaseToSentence(statusCodeEnum.ToString());

        return result;
    }

    private async Task MapAsync<T>(
        T vm,
        PageViewModelBuilderParameters mappingParameters
        )
        where T : IEditablePageViewModel, IPageRoutableViewModel
    {
        ArgumentNullException.ThrowIfNull(mappingParameters);
        ArgumentNullException.ThrowIfNull(mappingParameters.PageModel);

        vm.Page = mappingParameters.PageModel;
        vm.PageRoutingHelper = await CreatePageRoutingHelperAsync(mappingParameters);
        vm.IsPageEditMode = mappingParameters.VisualEditorMode == VisualEditorMode.Edit;
    }

    private async Task<PageRoutingHelper> CreatePageRoutingHelperAsync(
        PageViewModelBuilderParameters mappingParameters
        )
    {
        var allRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery());
        var allDirectories = await _queryExecutor.ExecuteAsync(new GetAllPageDirectoryRoutesQuery());
        var currentRoute = allRoutes.Single(p => p.PageId == mappingParameters.PageModel.PageId);

        var router = new PageRoutingHelper(
            allRoutes,
            allDirectories,
            currentRoute,
            mappingParameters.VisualEditorMode,
            mappingParameters.PageModel.PageVersionId
            );

        return router;
    }
}
