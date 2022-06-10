using System.Net;

namespace Cofoundry.Web;

/// <inheritdoc/>
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

    public virtual Task MapPageViewModelAsync(
        IPageViewModel viewModel,
        PageViewModelBuilderParameters mappingParameters
        )
    {
        return MapAsync(viewModel, mappingParameters);
    }

    public virtual async Task MapCustomEntityViewModelAsync<TDisplayModel>(
        ICustomEntityPageViewModel<TDisplayModel> viewModel,
        CustomEntityPageViewModelBuilderParameters mappingParameters
        ) where TDisplayModel : ICustomEntityPageDisplayModel
    {
        await MapAsync(viewModel, mappingParameters);

        ArgumentNullException.ThrowIfNull(mappingParameters.CustomEntityModel);

        var customEntityRenderDetails = mappingParameters.CustomEntityModel;
        var publishStatusQuery = mappingParameters.VisualEditorMode.ToPublishStatusQuery();

        var customModel = new CustomEntityRenderDetailsViewModel<TDisplayModel>();
        customModel.CustomEntityId = customEntityRenderDetails.CustomEntityId;
        customModel.CustomEntityVersionId = customEntityRenderDetails.CustomEntityVersionId;
        customModel.Locale = customEntityRenderDetails.Locale;
        customModel.Regions = customEntityRenderDetails.Regions;
        customModel.Title = customEntityRenderDetails.Title;
        customModel.UrlSlug = customEntityRenderDetails.UrlSlug;
        customModel.WorkFlowStatus = customEntityRenderDetails.WorkFlowStatus;
        customModel.PublishDate = customEntityRenderDetails.PublishDate;
        customModel.PublishStatus = customEntityRenderDetails.PublishStatus;
        customModel.CreateDate = customEntityRenderDetails.CreateDate;
        customModel.PageUrls = customEntityRenderDetails.PageUrls;

        customModel.Model = await _customEntityDisplayModelMapper.MapDisplayModelAsync<TDisplayModel>(customEntityRenderDetails, publishStatusQuery);

        viewModel.CustomEntity = customModel;
    }

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

    public virtual Task MapErrorPageViewModelAsync(
        IErrorPageViewModel viewModel,
        ErrorPageViewModelBuilderParameters mappingParameters
        )
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(mappingParameters);
        if (mappingParameters.StatusCode < 100) throw new ArgumentOutOfRangeException(nameof(mappingParameters.StatusCode));

        viewModel.StatusCode = mappingParameters.StatusCode;
        viewModel.StatusCodeDescription = GetStatusCodeDescription(viewModel.StatusCode);
        viewModel.PageTitle = "Error: " + viewModel.StatusCodeDescription;
        viewModel.MetaDescription = "Sorry, an error has occurred";
        viewModel.Path = mappingParameters.Path;
        viewModel.PathBase = mappingParameters.PathBase;
        viewModel.QueryString = mappingParameters.QueryString;

        return Task.CompletedTask;
    }

    private string GetStatusCodeDescription(int statusCode)
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