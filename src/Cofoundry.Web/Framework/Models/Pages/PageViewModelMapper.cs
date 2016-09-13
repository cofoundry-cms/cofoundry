using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class PageViewModelMapper : IPageViewModelMapper
    {
        #region constructor

        private static readonly MethodInfo _mapMethod = typeof(PageViewModelMapper).GetMethod("MapCustomEntityModel", BindingFlags.NonPublic | BindingFlags.Instance);
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

        public PageViewModel Map(PageRenderDetails page, SiteViewerMode siteViewerMode)
        {
            return Map<PageViewModel>(page, siteViewerMode);
        }

        public IEditablePageViewModel MapCustomEntityModel(
            Type displayModelType, 
            PageRenderDetails page, 
            CustomEntityRenderDetails customEntityRenderDetails, 
            SiteViewerMode siteViewerMode)
        {
            return (IEditablePageViewModel)_mapMethod
                .MakeGenericMethod(displayModelType)
                .Invoke(this, new object[] { page, customEntityRenderDetails, siteViewerMode });
        }

        public T Map<T>(PageRenderDetails page, SiteViewerMode siteViewerMode) where T : IEditablePageViewModel, IPageRoutableViewModel, new()
        {
            var vm = new T();
            vm.Page = page;
            vm.PageRoutingHelper = CreatePageRoutingHelper(page.PageId, page.PageVersionId, siteViewerMode);
            vm.IsPageEditMode = siteViewerMode == SiteViewerMode.Edit;

            return vm;
        }

        #endregion

        #region helpers

        private CustomEntityDetailsPageViewModel<TDisplayModel> MapCustomEntityModel<TDisplayModel>(
            PageRenderDetails page,
            CustomEntityRenderDetails customEntityRenderDetails,
            SiteViewerMode siteViewerMode
            ) where TDisplayModel : ICustomEntityDetailsDisplayViewModel
        {
            var vm = Map<CustomEntityDetailsPageViewModel<TDisplayModel>>(page, siteViewerMode);

            var customModel = new CustomEntityRenderDetailsViewModel<TDisplayModel>();
            customModel.CustomEntityId = customEntityRenderDetails.CustomEntityId;
            customModel.CustomEntityVersionId = customEntityRenderDetails.CustomEntityVersionId;
            customModel.Locale = customEntityRenderDetails.Locale;
            customModel.Sections = customEntityRenderDetails.Sections;
            customModel.Title = customEntityRenderDetails.Title;
            customModel.UrlSlug = customEntityRenderDetails.UrlSlug;
            customModel.WorkFlowStatus = customEntityRenderDetails.WorkFlowStatus;
            customModel.Model = _customEntityDisplayModelMapper.MapDetails<TDisplayModel>(customEntityRenderDetails);

            vm.CustomEntity = customModel;

            return vm;
        }

        private PageRoutingHelper CreatePageRoutingHelper(int pageId, int pageVersionId, SiteViewerMode siteViewerMode)
        {
            var allRoutes = _queryExecutor.GetAll<PageRoute>();
            var allDirectories = _queryExecutor.GetAll<WebDirectoryRoute>();
            var currentRoute = allRoutes.Single(p => p.PageId == pageId);

            var router = new PageRoutingHelper(allRoutes, allDirectories, currentRoute, siteViewerMode, pageVersionId);
            return router;
        }


        #endregion
    }
}
