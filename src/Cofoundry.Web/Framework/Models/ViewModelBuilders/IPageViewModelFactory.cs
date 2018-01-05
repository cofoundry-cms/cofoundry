using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory for creating view models for the various page types
    /// used by the Cofoundry dynamic page system. Override the base 
    /// implementation to provide your own base view model page types.
    /// </summary>
    public interface IPageViewModelFactory
    {
        /// <summary>
        /// Creates a new view model for a generic page.
        /// </summary>
        IPageViewModel CreatePageViewModel();

        /// <summary>
        /// Creates a new view model for a custom entity details page.
        /// </summary>
        /// <typeparam name="TDisplayModel">The type of custom entity display model.</typeparam>
        ICustomEntityPageViewModel<TDisplayModel> CreateCustomEntityPageViewModel<TDisplayModel>()
            where TDisplayModel : ICustomEntityPageDisplayModel;

        /// <summary>
        /// Creates a new view model for a 404 page.
        /// </summary>
        INotFoundPageViewModel CreateNotFoundPageViewModel();

        /// <summary>
        /// Creates a new view model for a generi error page.
        /// </summary>
        IErrorPageViewModel CreateErrorPageViewModel();
    }
}