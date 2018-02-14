using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using System.Net;
using Cofoundry.Core;

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
        /// Maps data to an empty view model for a custom entity page.
        /// </summary>
        /// <param name="displayModelType">The type information of the display model to apply to the generic view model.</param>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual async Task MapCustomEntityViewModelAsync<TDisplayModel>(
            ICustomEntityPageViewModel<TDisplayModel> viewModel,
            CustomEntityPageViewModelBuilderParameters mappingParameters 
            ) where TDisplayModel : ICustomEntityPageDisplayModel
        {
            await MapAsync(viewModel, mappingParameters);

            if (mappingParameters.CustomEntityModel == null) throw new ArgumentNullException(nameof(mappingParameters.CustomEntityModel));

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

        /// <summary>
        /// Maps data to an empty view model for a 404 page.
        /// </summary>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual Task MapNotFoundPageViewModelAsync(
            INotFoundPageViewModel viewModel,
            NotFoundPageViewModelBuilderParameters mappingParameters
            )
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (mappingParameters == null) throw new ArgumentNullException(nameof(mappingParameters));

            viewModel.PageTitle = "Not found";
            viewModel.MetaDescription = "Sorry, that page could not be found";
            viewModel.Path = mappingParameters.Path;
            viewModel.PathBase = mappingParameters.PathBase;
            viewModel.QueryString = mappingParameters.QueryString;
            viewModel.StatusCode = 404;
            viewModel.StatusCodeDescription = "Not Found";

            return Task.CompletedTask;
        }

        /// <summary>
        /// Maps data to an empty view model for a generic error page.
        /// </summary>
        /// <param name="viewModel">The view model to map data to.</param>
        /// <param name="mappingParameters">The data passed through to map to the view model.</param>
        public virtual Task MapErrorPageViewModelAsync(
            IErrorPageViewModel viewModel,
            ErrorPageViewModelBuilderParameters mappingParameters
            )
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (mappingParameters == null) throw new ArgumentNullException(nameof(mappingParameters));
            if (mappingParameters.StatusCode < 100) throw new ArgumentOutOfRangeException(nameof(mappingParameters.StatusCode));

            viewModel.StatusCode = mappingParameters.StatusCode;
            viewModel.StatusCodeDescription = GetStatusCodeDescription(viewModel.StatusCode);
            viewModel.PageTitle = "Error: " + viewModel.StatusCodeDescription;
            viewModel.MetaDescription = "Sorry, an error has occured";
            viewModel.Path = mappingParameters.Path;
            viewModel.PathBase = mappingParameters.PathBase;
            viewModel.QueryString = mappingParameters.QueryString;

            return Task.CompletedTask;
        }

        #endregion

        #region helpers

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

        #endregion
    }
}
