﻿namespace Cofoundry.Web;

/// <summary>
/// Creates and maps raw data to the view models used by the Cofoundry dynamic page 
/// system. You can override the default implementation to customize the view model types
/// and mapping behaviour.
/// </summary>
public interface IPageViewModelBuilder
{
    /// <summary>
    /// Creates and maps data to a view model for a generic page.
    /// </summary>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task<IPageViewModel> BuildPageViewModelAsync(
        PageViewModelBuilderParameters mappingParameters
        );

    /// <summary>
    /// Creates and maps data to a view model for a custom entity details page.
    /// </summary>
    /// <typeparam name="TDisplayModel">The type of display model to apply to the generic view model.</typeparam>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task<ICustomEntityPageViewModel<TDisplayModel>> BuildCustomEntityPageViewModelAsync<TDisplayModel>(
        CustomEntityPageViewModelBuilderParameters mappingParameters
        ) where TDisplayModel : ICustomEntityPageDisplayModel;

    /// <summary>
    /// Creates and maps data to a view model for a 404 page.
    /// </summary>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task<INotFoundPageViewModel> BuildNotFoundPageViewModelAsync(
        NotFoundPageViewModelBuilderParameters mappingParameters
        );

    /// <summary>
    /// Creates and maps data to a view model for a generic error page.
    /// </summary>
    /// <param name="mappingParameters">The data passed through to map to the view model.</param>
    Task<IErrorPageViewModel> BuildErrorPageViewModelAsync(
        ErrorPageViewModelBuilderParameters mappingParameters
        );
}
