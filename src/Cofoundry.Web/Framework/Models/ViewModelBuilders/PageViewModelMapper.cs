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
    /// Maps raw data to the view models used by the Cofoundry dynamic page 
    /// system. You can override this implementation to customize the mapping 
    /// behaviour.
    /// </summary>
    /// <remarks>
    /// Note that we use async methods here not because we need it in this implementation
    /// but because it may be needed for an overriding class where data access may be 
    /// neccessary.
    /// </remarks>
    public class PageViewModelMapper : IPageViewModelMapper
    {
        #region constructor

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

        #endregion

        #region public methods

        /// <summary>
        /// Maps data to an empty view model for a generic page.
        /// </summary>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual Task MapPageViewModelAsync(
            IPageViewModel viewModel,
            PageViewModelBuilderParameters mappingParameters
            )
        {
            return MapAsync(viewModel, mappingParameters);
        }

        /// <summary>
        /// Maps data to an empty view model for a custom entity details page.
        /// </summary>
        /// <param name="displayModelType">The type information of the display model to apply to the generic view model.</param>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual async Task MapCustomEntityViewModelAsync<TDisplayModel>(
            ICustomEntityDetailsPageViewModel<TDisplayModel> viewModel,
            CustomEntityDetailsPageViewModelBuilderParameters mappingParameters 
            ) where TDisplayModel : ICustomEntityDetailsDisplayViewModel
        {
            await MapAsync(viewModel, mappingParameters);

            if (mappingParameters.CustomEntityModel == null) throw new ArgumentNullException(nameof(mappingParameters.CustomEntityModel));

            var customEntityRenderDetails = mappingParameters.CustomEntityModel;

            var customModel = new CustomEntityRenderDetailsViewModel<TDisplayModel>();
            customModel.CustomEntityId = customEntityRenderDetails.CustomEntityId;
            customModel.CustomEntityVersionId = customEntityRenderDetails.CustomEntityVersionId;
            customModel.Locale = customEntityRenderDetails.Locale;
            customModel.Sections = customEntityRenderDetails.Sections;
            customModel.Title = customEntityRenderDetails.Title;
            customModel.UrlSlug = customEntityRenderDetails.UrlSlug;
            customModel.WorkFlowStatus = customEntityRenderDetails.WorkFlowStatus;
            customModel.Model = await _customEntityDisplayModelMapper.MapDetailsAsync<TDisplayModel>(customEntityRenderDetails);

            viewModel.CustomEntity = customModel;
        }

        /// <summary>
        ///Maps data to an empty view model for a 404 page.
        /// </summary>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual Task MapNotFoundPageViewModelAsync(
            INotFoundPageViewModel viewModel,
            NotFoundPageViewModelBuilderParameters mappingParameters
            )
        {
            viewModel.PageTitle = "Page not found";
            viewModel.MetaDescription = "Sorry, that page could not be found";

            return Task.CompletedTask;
        }

        #endregion

        #region helpers

        private async Task MapAsync<T>(
            T vm, 
            PageViewModelBuilderParameters mappingParameters
            )
            where T : IEditablePageViewModel, IPageRoutableViewModel
        {
            if (mappingParameters == null) throw new ArgumentNullException(nameof(mappingParameters));
            if (mappingParameters.PageModel == null) throw new ArgumentNullException(nameof(mappingParameters.PageModel));

            vm.Page = mappingParameters.PageModel;
            vm.PageRoutingHelper = await CreatePageRoutingHelperAsync(mappingParameters);
            vm.IsPageEditMode = mappingParameters.VisualEditorMode == VisualEditorMode.Edit;
        }

        private async Task<PageRoutingHelper> CreatePageRoutingHelperAsync(
            PageViewModelBuilderParameters mappingParameters
            )
        {
            var allRoutes = await _queryExecutor.GetAllAsync<PageRoute>();
            var allDirectories = await _queryExecutor.GetAllAsync<WebDirectoryRoute>();
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

        #endregion
    }
}
