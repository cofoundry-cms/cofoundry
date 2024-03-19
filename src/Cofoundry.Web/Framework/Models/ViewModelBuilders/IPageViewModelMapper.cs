﻿namespace Cofoundry.Web;

/// <summary>
/// Maps raw data to the view models used by the Cofoundry dynamic page 
/// system. You can override the default implementation to customize the mapping 
/// behaviour.
/// </summary>
public interface IPageViewModelMapper
{
    /// <summary>
    /// Maps data to an empty view model for a generic page.
    /// </summary>
    /// <param name="viewModel">The view model to map data to.</param>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task MapPageViewModelAsync(
        IPageViewModel viewModel,
        PageViewModelBuilderParameters mappingParameters
        );

    /// <summary>
    /// Maps data to an empty view model for a custom entity details page.
    /// </summary>
    /// <param name="displayModelType">The type information of the display model to apply to the generic view model.</param>
    /// <param name="viewModel">The view model to map data to.</param>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task MapCustomEntityViewModelAsync<TDisplayModel>(
        ICustomEntityPageViewModel<TDisplayModel> viewModel,
        CustomEntityPageViewModelBuilderParameters mappingParameters
        ) where TDisplayModel : ICustomEntityPageDisplayModel;

    /// <summary>
    /// Maps data to an empty view model for a 404 page.
    /// </summary>
    /// <param name="viewModel">The view model to map data to.</param>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task MapNotFoundPageViewModelAsync(
        INotFoundPageViewModel viewModel,
        NotFoundPageViewModelBuilderParameters mappingParameters
        );

    /// <summary>
    /// Maps data to an empty view model for a generic error page.
    /// </summary>
    /// <param name="viewModel">The view model to map data to.</param>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task MapErrorPageViewModelAsync(
        IErrorPageViewModel viewModel,
        ErrorPageViewModelBuilderParameters mappingParameters
        );
}
