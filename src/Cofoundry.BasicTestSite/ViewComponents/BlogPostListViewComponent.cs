using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class BlogPostListViewComponent : ViewComponent
    {
        private readonly ICustomEntityRepository _customEntityRepository;
        private readonly IImageAssetRepository _imageAssetRepository;
        private readonly IPageResponseDataCache _pageRenderDataCache;
        private readonly IVisualEditorStateService _visualEditorStateService;

        public BlogPostListViewComponent(
            ICustomEntityRepository customEntityRepository,
            IImageAssetRepository imageAssetRepository,
            IPageResponseDataCache pageRenderDataCache,
            IVisualEditorStateService visualEditorStateService
            )
        {
            _customEntityRepository = customEntityRepository;
            _imageAssetRepository = imageAssetRepository;
            _pageRenderDataCache = pageRenderDataCache;
            _visualEditorStateService = visualEditorStateService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // TODO: no model binder access....
            // https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Core/ModelBinding/Internal/ModelBindingHelper.cs
            //HttpContext.Request.
            var webQuery = new SearchBlogPostsQuery();
            //---------

            var query = new SearchCustomEntityRenderSummariesQuery();
            query.CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode;
            query.PageNumber = webQuery.PageNumber;
            query.PageSize = 30;

            // Publish status defaults to live, but we can use the current visual editor
            // state to allow us to show draft blog posts when previewing a draft page.
            var state = await _visualEditorStateService.GetCurrentAsync();
            query.PublishStatus = state.GetAmbientEntityPublishStatusQuery();

            // TODO: Filtering by Category (webQuery.CategoryId)
            // Searching/filtering custom entities is not implemented yet, but it
            // is possible to build your own search index using the message handling
            // framework or writing a custom query against the UnstructuredDataDependency table

            var entities = await _customEntityRepository.SearchCustomEntityRenderSummariesAsync(query);
            var viewModel = await MapBlogPostsAsync(entities);

            return View(viewModel);
        }

        private async Task<PagedQueryResult<BlogPostSummary>> MapBlogPostsAsync(PagedQueryResult<CustomEntityRenderSummary> customEntityResult)
        {
            var blogPosts = new List<BlogPostSummary>(customEntityResult.Items.Count());

            var imageAssetIds = customEntityResult
                .Items
                .Select(i => (BlogPostDataModel)i.Model)
                .Select(m => m.ThumbnailImageAssetId)
                .Distinct();

            var images = await _imageAssetRepository.GetImageAssetRenderDetailsByIdRangeAsync(imageAssetIds);

            foreach (var customEntity in customEntityResult.Items)
            {
                var model = (BlogPostDataModel)customEntity.Model;

                var blogPost = new BlogPostSummary();
                blogPost.Title = customEntity.Title;
                blogPost.ShortDescription = model.ShortDescription;
                blogPost.ThumbnailImageAsset = images.GetOrDefault(model.ThumbnailImageAssetId);
                blogPost.FullPath = customEntity.PageUrls.FirstOrDefault();
                blogPost.PostDate = customEntity.CreateDate;

                blogPosts.Add(blogPost);
            }

            return customEntityResult.ChangeType(blogPosts);
        }
    }
}
