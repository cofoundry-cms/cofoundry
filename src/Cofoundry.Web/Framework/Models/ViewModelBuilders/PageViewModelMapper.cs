using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Conditions;

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
        public virtual void MapPageViewModel(
            IPageViewModel viewModel,
            PageViewModelBuilderParameters mappingParameters
            )
        {
            Map(viewModel, mappingParameters);
        }

        /// <summary>
        /// Maps data to an empty view model for a custom entity details page.
        /// </summary>
        /// <param name="displayModelType">The type information of the display model to apply to the generic view model.</param>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual void MapCustomEntityViewModel<TDisplayModel>(
            ICustomEntityDetailsPageViewModel<TDisplayModel> viewModel,
            CustomEntityDetailsPageViewModelBuilderParameters mappingParameters 
            ) where TDisplayModel : ICustomEntityDetailsDisplayViewModel
        {
            Map(viewModel, mappingParameters);

            Condition.Requires(mappingParameters.CustomEntityModel).IsNotNull();

            var customEntityRenderDetails = mappingParameters.CustomEntityModel;

            var customModel = new CustomEntityRenderDetailsViewModel<TDisplayModel>();
            customModel.CustomEntityId = customEntityRenderDetails.CustomEntityId;
            customModel.CustomEntityVersionId = customEntityRenderDetails.CustomEntityVersionId;
            customModel.Locale = customEntityRenderDetails.Locale;
            customModel.Sections = customEntityRenderDetails.Sections;
            customModel.Title = customEntityRenderDetails.Title;
            customModel.UrlSlug = customEntityRenderDetails.UrlSlug;
            customModel.WorkFlowStatus = customEntityRenderDetails.WorkFlowStatus;
            customModel.Model = _customEntityDisplayModelMapper.MapDetails<TDisplayModel>(customEntityRenderDetails);

            viewModel.CustomEntity = customModel;
        }

        /// <summary>
        ///Maps data to an empty view model for a 404 page.
        /// </summary>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual void MapNotFoundPageViewModel(
            INotFoundPageViewModel viewModel,
            NotFoundPageViewModelBuilderParameters mappingParameters
            )
        {
            viewModel.PageTitle = "Page not found";
            viewModel.MetaDescription = "Sorry, that page could not be found";
        }

        #endregion

        #region helpers

        private void Map<T>(
            T vm, 
            PageViewModelBuilderParameters mappingParameters
            )
            where T : IEditablePageViewModel, IPageRoutableViewModel
        {
            Condition.Requires(mappingParameters).IsNotNull();
            Condition.Requires(mappingParameters.PageModel).IsNotNull();

            vm.Page = mappingParameters.PageModel;
            vm.PageRoutingHelper = CreatePageRoutingHelper(mappingParameters);
            vm.IsPageEditMode = mappingParameters.VisualEditorMode == VisualEditorMode.Edit;
        }

        private PageRoutingHelper CreatePageRoutingHelper(
            PageViewModelBuilderParameters mappingParameters
            )
        {
            var allRoutes = _queryExecutor.GetAll<PageRoute>();
            var allDirectories = _queryExecutor.GetAll<WebDirectoryRoute>();
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
