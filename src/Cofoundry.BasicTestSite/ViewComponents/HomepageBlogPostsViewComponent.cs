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
    public class HomepageBlogPostsViewComponent : ViewComponent
    {
        private readonly ICustomEntityRepository _customEntityRepository;
        private readonly IImageAssetRepository _imageAssetRepository;
        private readonly IVisualEditorStateService _visualEditorStateService;

        public HomepageBlogPostsViewComponent(
            ICustomEntityRepository customEntityRepository,
            IImageAssetRepository imageAssetRepository,
            IVisualEditorStateService visualEditorStateService
            )
        {
            _customEntityRepository = customEntityRepository;
            _imageAssetRepository = imageAssetRepository;
            _visualEditorStateService = visualEditorStateService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var visualEditorState = await _visualEditorStateService.GetCurrentAsync();

            var query = new SearchCustomEntityRenderSummariesQuery();
            query.CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode;
            query.PublishStatus = visualEditorState.GetAmbientEntityPublishStatusQuery();
            query.PageSize = 3;

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
