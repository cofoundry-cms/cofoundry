using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Creates and maps raw data to the view models used by the Cofoundry dynamic page 
    /// system. You can override this implementation to customize the view model types
    /// and mapping behaviour.
    /// </summary>
    /// <remarks>
    /// Note that we use async methods here not because we need it in this implementation
    /// but because it may be needed for an overriding class where data access may be 
    /// neccessary.
    /// </remarks>
    public class PageViewModelBuilder : IPageViewModelBuilder
    {
        #region constructor

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

        #endregion

        #region public methods

        /// <summary>
        /// Creates and maps data to a view model for a generic page.
        /// </summary>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual async Task<IPageViewModel> BuildPageViewModelAsync(
            PageViewModelBuilderParameters mappingParameters
            )
        {
            var viewModel = _pageViewModelFactory.CreatePageViewModel();

            await _pageViewModelMapper.MapPageViewModelAsync(viewModel, mappingParameters);

            return viewModel;
        }

        /// <summary>
        /// Creates and maps data to a view model for a custom entity details page.
        /// </summary>
        /// <typeparam name="TDisplayModel">The type of display model to apply to the generic view model.</typeparam>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual async Task<ICustomEntityPageViewModel<TDisplayModel>> BuildCustomEntityPageViewModelAsync<TDisplayModel>(
            CustomEntityPageViewModelBuilderParameters mappingParameters
            ) where TDisplayModel : ICustomEntityPageDisplayModel
        {
            var viewModel = _pageViewModelFactory.CreateCustomEntityPageViewModel<TDisplayModel>();

            await _pageViewModelMapper.MapCustomEntityViewModelAsync(viewModel, mappingParameters);

            return viewModel;
        }

        /// <summary>
        /// Creates and maps data to a view model for a 404 page.
        /// </summary>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual async Task<INotFoundPageViewModel> BuildNotFoundPageViewModelAsync(
            NotFoundPageViewModelBuilderParameters mappingParameters
            )
        {
            var viewModel = _pageViewModelFactory.CreateNotFoundPageViewModel();

            await _pageViewModelMapper.MapNotFoundPageViewModelAsync(viewModel, mappingParameters);

            return viewModel;
        }

        /// <summary>
        /// Creates and maps data to a view model for a generic error page.
        /// </summary>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual async Task<IErrorPageViewModel> BuildErrorPageViewModelAsync(
            ErrorPageViewModelBuilderParameters mappingParameters
            )
        {
            var viewModel = _pageViewModelFactory.CreateNotFoundPageViewModel();

            await _pageViewModelMapper.MapErrorPageViewModelAsync(viewModel, mappingParameters);

            return viewModel;
        }

        #endregion
    }
}
