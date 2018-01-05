using Cofoundry.Domain;
using Cofoundry.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class ExamplePageViewModelBuilder : IPageViewModelBuilder
    {
        private readonly IPageViewModelFactory _pageViewModelFactory;
        private readonly IPageViewModelMapper _pageViewModelMapper;

        public ExamplePageViewModelBuilder(
            IPageViewModelFactory pageViewModelFactory,
            IPageViewModelMapper pageViewModelMapper
            )
        {
            // Constructor injection is supported
            // Here we make use of the same helpers used in the base class
            _pageViewModelFactory = pageViewModelFactory;
            _pageViewModelMapper = pageViewModelMapper;
        }


        public async Task<IPageViewModel> BuildPageViewModelAsync(PageViewModelBuilderParameters mappingParameters)
        {
            // Create the custom view model instance
            var viewModel = new ExamplePageViewModel();

            // Do the base mapping
            await _pageViewModelMapper.MapPageViewModelAsync(viewModel, mappingParameters);

            // TODO: insert your custom custom mapping

            return viewModel;
        }

        public async Task<ICustomEntityPageViewModel<TDisplayModel>> BuildCustomEntityPageViewModelAsync<TDisplayModel>(
           CustomEntityPageViewModelBuilderParameters mappingParameters
           ) where TDisplayModel : ICustomEntityPageDisplayModel
        {
            // Create the custom view model instance
            var viewModel = new ExampleCustomEntityPageViewModel<TDisplayModel>();

            // Do the base mapping
            await _pageViewModelMapper.MapCustomEntityViewModelAsync(viewModel, mappingParameters);

            // Example of calling an async custom mapping function
            await ExampleCustomMappingAsync(viewModel);

            return viewModel;
        }

        public async Task<INotFoundPageViewModel> BuildNotFoundPageViewModelAsync(NotFoundPageViewModelBuilderParameters mappingParameters)
        {
            // This example show using the default behaviour without any customization
            // You could alternatively inherit from PageViewModelBuilder and use the base implementation
            var viewModel = _pageViewModelFactory.CreateNotFoundPageViewModel();

            await _pageViewModelMapper.MapNotFoundPageViewModelAsync(viewModel, mappingParameters);

            return viewModel;
        }

        private Task ExampleCustomMappingAsync<TDisplayModel>(ICustomEntityPageViewModel<TDisplayModel> model)
            where TDisplayModel : ICustomEntityPageDisplayModel
        {
            return Task.CompletedTask;
        }

        public async Task<IErrorPageViewModel> BuildErrorPageViewModelAsync(ErrorPageViewModelBuilderParameters mappingParameters)
        {
            // This example show using the default behaviour without any customization
            // You could alternatively inherit from PageViewModelBuilder and use the base implementation
            var viewModel = _pageViewModelFactory.CreateErrorPageViewModel();

            await _pageViewModelMapper.MapErrorPageViewModelAsync(viewModel, mappingParameters);

            return viewModel;
        }
    }
}