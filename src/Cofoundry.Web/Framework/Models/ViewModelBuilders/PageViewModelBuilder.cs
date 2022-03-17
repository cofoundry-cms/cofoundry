namespace Cofoundry.Web;

/// <inheritdoc/>
public class PageViewModelBuilder : IPageViewModelBuilder
{
    private readonly IPageViewModelMapper _pageViewModelMapper;
    private readonly IPageViewModelFactory _pageViewModelFactory;

    public PageViewModelBuilder(
        IPageViewModelMapper pageViewModelMapper,
        IPageViewModelFactory pageViewModelFactory
        )
    {
        _pageViewModelMapper = pageViewModelMapper;
        _pageViewModelFactory = pageViewModelFactory;
    }

    public virtual async Task<IPageViewModel> BuildPageViewModelAsync(
        PageViewModelBuilderParameters mappingParameters
        )
    {
        var viewModel = _pageViewModelFactory.CreatePageViewModel();

        await _pageViewModelMapper.MapPageViewModelAsync(viewModel, mappingParameters);

        return viewModel;
    }

    public virtual async Task<ICustomEntityPageViewModel<TDisplayModel>> BuildCustomEntityPageViewModelAsync<TDisplayModel>(
        CustomEntityPageViewModelBuilderParameters mappingParameters
        ) where TDisplayModel : ICustomEntityPageDisplayModel
    {
        var viewModel = _pageViewModelFactory.CreateCustomEntityPageViewModel<TDisplayModel>();

        await _pageViewModelMapper.MapCustomEntityViewModelAsync(viewModel, mappingParameters);

        return viewModel;
    }

    public virtual async Task<INotFoundPageViewModel> BuildNotFoundPageViewModelAsync(
        NotFoundPageViewModelBuilderParameters mappingParameters
        )
    {
        var viewModel = _pageViewModelFactory.CreateNotFoundPageViewModel();

        await _pageViewModelMapper.MapNotFoundPageViewModelAsync(viewModel, mappingParameters);

        return viewModel;
    }

    public virtual async Task<IErrorPageViewModel> BuildErrorPageViewModelAsync(
        ErrorPageViewModelBuilderParameters mappingParameters
        )
    {
        var viewModel = _pageViewModelFactory.CreateNotFoundPageViewModel();

        await _pageViewModelMapper.MapErrorPageViewModelAsync(viewModel, mappingParameters);

        return viewModel;
    }
}