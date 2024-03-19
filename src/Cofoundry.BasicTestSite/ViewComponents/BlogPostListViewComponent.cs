using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.BasicTestSite;

public class BlogPostListViewComponent : ViewComponent
{
    private readonly IContentRepository _contentRepository;
    private readonly IVisualEditorStateService _visualEditorStateService;

    public BlogPostListViewComponent(
        IContentRepository contentRepository,
        IVisualEditorStateService visualEditorStateService
        )
    {
        _contentRepository = contentRepository;
        _visualEditorStateService = visualEditorStateService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // TODO: no model binder access....
        // https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Core/ModelBinding/Internal/ModelBindingHelper.cs
        //HttpContext.Request.
        var webQuery = new SearchBlogPostsQuery();
        //---------

        // Publish status defaults to live, but we can use the current visual editor
        // state to allow us to show draft blog posts when previewing a draft page.
        var state = await _visualEditorStateService.GetCurrentAsync();

        // TODO: Filtering by Category (webQuery.CategoryId)
        // Searching/filtering custom entities is not implemented yet, but it
        // is possible to build your own search index using the message handling
        // framework or writing a custom query against the UnstructuredDataDependency table

        var entities = await _contentRepository
            .CustomEntities()
            .Search()
            .AsRenderSummaries(new()
            {
                CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode,
                PageNumber = webQuery.PageNumber,
                PageSize = 30,
                PublishStatus = state.GetAmbientEntityPublishStatusQuery()
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

            var blogPost = new BlogPostSummary()
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
