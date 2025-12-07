using Microsoft.AspNetCore.Mvc;

namespace SimpleSite;

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
        // We can use the current visual editor state (e.g. edit mode, live mode) to
        // determine whether to show unpublished blog posts in the list.
        var visualEditorState = await _visualEditorStateService.GetCurrentAsync();
        var ambientEntityPublishStatusQuery = visualEditorState.GetAmbientEntityPublishStatusQuery();

        var query = new SearchCustomEntityRenderSummariesQuery()
        {
            CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode,
            PageSize = 3,
            PublishStatus = ambientEntityPublishStatusQuery
        };

        var entities = await _contentRepository
            .CustomEntities()
            .Search()
            .AsRenderSummaries(query)
            .ExecuteAsync();

        var viewModel = await MapBlogPostsAsync(entities, ambientEntityPublishStatusQuery);

        return View(viewModel);
    }

    /// <summary>
    /// Here we map the raw custom entity data from Cofoundry into our
    /// own BlogPostSummary which will get sent to be rendered in the 
    /// view.
    /// 
    /// This code is repeated in BlogPostListViewComponent for 
    /// simplicity, but typically you'd put the code into a shared 
    /// mapper or break data access out into it's own shared layer.
    /// </summary>
    private async Task<PagedQueryResult<BlogPostSummary>> MapBlogPostsAsync(
        PagedQueryResult<CustomEntityRenderSummary> customEntityResult,
        PublishStatusQuery ambientEntityPublishStatusQuery
        )
    {
        var blogPosts = new List<BlogPostSummary>(customEntityResult.Items.Count);

        var imageAssetIds = customEntityResult
            .Items
            .Select(i => (BlogPostDataModel)i.Model)
            .Select(m => m.ThumbnailImageAssetId)
            .Distinct();

        var authorIds = customEntityResult
            .Items
            .Select(i => (BlogPostDataModel)i.Model)
            .Select(m => m.AuthorId)
            .Distinct();

        var imageLookup = await _contentRepository
            .ImageAssets()
            .GetByIdRange(imageAssetIds)
            .AsRenderDetails()
            .ExecuteAsync();

        var authorLookup = await _contentRepository
            .CustomEntities()
            .GetByIdRange(authorIds)
            .AsRenderSummaries(ambientEntityPublishStatusQuery)
            .ExecuteAsync();

        foreach (var customEntity in customEntityResult.Items)
        {
            var model = (BlogPostDataModel)customEntity.Model;
            var author = authorLookup.GetValueOrDefault(model.AuthorId);

            var blogPost = new BlogPostSummary()
            {
                Title = customEntity.Title,
                ShortDescription = model.ShortDescription,
                ThumbnailImageAsset = imageLookup.GetValueOrDefault(model.ThumbnailImageAssetId),
                FullPath = customEntity.PageUrls.FirstOrDefault() ?? string.Empty,
                PostDate = customEntity.PublishDate,
                AuthorName = author?.Title
            };

            blogPosts.Add(blogPost);
        }

        return customEntityResult.ChangeType(blogPosts);
    }
}
