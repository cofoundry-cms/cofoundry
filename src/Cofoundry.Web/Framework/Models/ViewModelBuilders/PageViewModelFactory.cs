using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory for creating view models for the various page types
    /// used by the Cofoundry dynamic page system. Override this implementation
    /// to provide your own base view model page types.
    /// </summary>
    public class PageViewModelFactory : IPageViewModelFactory
    {
        /// <summary>
        /// Creates a new view model for a generic page.
        /// </summary>
        public virtual IPageViewModel CreatePageViewModel()
        {
            return new PageViewModel();
        }

        /// <summary>
        /// Creates a new view model for a custom entity details page.
        /// </summary>
        /// <typeparam name="TDisplayModel">The type of custom entity display model.</typeparam>
        public virtual ICustomEntityDetailsPageViewModel<TDisplayModel> CreateCustomEntityDetailsPageViewModel<TDisplayModel>()
            where TDisplayModel : ICustomEntityDetailsDisplayViewModel
        {
            return new CustomEntityDetailsPageViewModel<TDisplayModel>();
        }

        /// <summary>
        /// Creates a new view model for a 404 page.
        /// </summary>
        public INotFoundPageViewModel CreateNotFoundPageViewModel()
        {
            return new NotFoundPageViewModel();
        }
    }
}