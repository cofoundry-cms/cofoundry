using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        public BlogPostListViewComponent(
            ICustomEntityRepository customEntityRepository,
            IImageAssetRepository imageAssetRepository,
            IOptions<StaticFileOptions> staticFileOptions
            )
        {
            _customEntityRepository = customEntityRepository;
            _imageAssetRepository = imageAssetRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // TODO: YAH: no model binder access....
            // https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Core/ModelBinding/Internal/ModelBindingHelper.cs
            //HttpContext.Request.
            var webQuery = new SearchBlogPostsQuery();
            //---------

            var query = new SearchCustomEntityRenderSummariesQuery();
            query.CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode;
            query.PageNumber = webQuery.PageNumber;
            query.PageSize = 30;
            query.WorkFlowStatus = WorkFlowStatusQuery.Published;

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
                blogPost.FullPath = customEntity.DetailsPageUrls.FirstOrDefault();
                blogPost.PostDate = customEntity.CreateDate;

                blogPosts.Add(blogPost);
            }

            return customEntityResult.ChangeType(blogPosts);
        }
    }
}
