﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.BasicTestSite;

public class HomepageBlogPostsViewComponent : ViewComponent
{
    private readonly IContentRepository _contentRepository;
    private readonly IVisualEditorStateService _visualEditorStateService;

    public HomepageBlogPostsViewComponent(
        IContentRepository contentRepository,
        IVisualEditorStateService visualEditorStateService
        )
    {
        _contentRepository = contentRepository;
        _visualEditorStateService = visualEditorStateService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var visualEditorState = await _visualEditorStateService.GetCurrentAsync();

        var entities = await _contentRepository
            .CustomEntities()
            .Search()
            .AsRenderSummaries(new SearchCustomEntityRenderSummariesQuery()
            {
                CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode,
                PublishStatus = visualEditorState.GetAmbientEntityPublishStatusQuery(),
                PageSize = 3
            })
            .ExecuteAsync();

        var viewModel = await MapBlogPostsAsync(entities);

        return View(viewModel);
    }

    private async Task<PagedQueryResult<BlogPostSummary>> MapBlogPostsAsync(PagedQueryResult<CustomEntityRenderSummary> customEntityResult)
    {
        var blogPosts = new List<BlogPostSummary>(customEntityResult.Items.Count);

        var imageAssetIds = customEntityResult
            .Items
            .Select(i => (BlogPostDataModel)i.Model)
            .Select(m => m.ThumbnailImageAssetId)
            .Distinct();

        var images = await _contentRepository
            .ImageAssets()
            .GetByIdRange(imageAssetIds)
            .AsRenderDetails()
            .ExecuteAsync();

        foreach (var customEntity in customEntityResult.Items)
        {
            var model = (BlogPostDataModel)customEntity.Model;

            var blogPost = new BlogPostSummary
            {
                Title = customEntity.Title,
                ShortDescription = model.ShortDescription,
                ThumbnailImageAsset = images.GetValueOrDefault(model.ThumbnailImageAssetId),
                FullPath = customEntity.PageUrls.FirstOrDefault(),
                PostDate = customEntity.CreateDate
            };

            blogPosts.Add(blogPost);
        }

        return customEntityResult.ChangeType(blogPosts);
    }
}
