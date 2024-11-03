Both pages and custom entities support versioning, so you work on an entity in draft status before publishing it to the live site. A historical record of changes are kept so that you can revert to a previous version if need be.

## Working with versioned entities

For the most part versioning is managed automatically. The admin UI will show the latest version and when editing pages in the visual editor Cofoundry will load the correct version and display the block data associated with the version being edited.

For parts of your site where you're manually loading data you'll want to make sure you're loading a version of the data that the user expects to see.

Examples of this might be for relation data manually loaded into a page block, or perhaps a standalone part of your site like a navigation menu loaded via a ViewComponent.

### PublishStatusQuery

Cofoundry queries that support versioned data will usually include a `PublishStatusQuery` parameter that you can use to load the appropriate version of your data. 

The good news is that the default value is always `PublishStatusQuery.Published` which ensures that you're never accidentally exposing unpublished data on your live site.

There's a few other values to the `PublishStatusQuery` enum, but the common one that you'll need to be aware of is `PublishStatusQuery.Latest` which can be used to get the latest version of an entity irrespective of whether it has been published or not.

### Querying related data from a page block mapper

Firstly we'll look at page blocks, which could load in page or custom entity data. I'll use the *Page Snippet* block type from the [PageBlockTypes sample project](https://github.com/cofoundry-cms/Cofoundry.Samples.PageBlockTypes) as an example.

In the display model mapper we need to load in the page that has been selected by the user by running a `GetPageRenderDetailsByIdRangeQuery`. Cofoundry makes this really simple for us here because the correct `PublishStatusQuery` for loading related entities is passed into the `MapAsync` method and all we need to do is pass it into our query constructor. 

```csharp
public class PageSnippetDisplayModelMapper : IPageBlockTypeDisplayModelMapper<PageSnippetDataModel>
{
    // … constructor ommited

    public async Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<PageSnippetDataModel> context,
            PageBlockTypeDisplayModelMapperResult<PageSnippetDataModel> result
        )
    {
        var allPageIds = context.Items.SelectDistinctModelValuesWithoutEmpty(m => m.PageId);

        // We pass through the PublishStatusQuery to ensure this is respected
        // when querying related data i.e. if we're viewing a draft version then
        // we should also be able to see connected entities in draft status.
        var allPages = await _contentRepository
                .Pages()
                .GetByIdRange(allPageIds)
                .AsRenderDetails(context.PublishStatusQuery)
                .ExecuteAsync();

        foreach (var item in context.Items)
        {
            var displayModel = new PageSnippetDisplayModel();

            displayModel.Page = allPages.GetOrDefault(item.DataModel.PageId);

            // We have to code defensively here and bear in mind that the related
            // entities may be in draft status and may not be available when viewing
            // the live site.
            if (displayModel.Page != null)
            {
                // … mapping ommited
            }

            result.Add(item, displayModel);
        }
    }
}
```

## Querying related data from a custom entity display model mapper

When mapping custom entity display models, the mapper works in exactly the same way as for block mappers, the correct PublishStatusQuery for related entities is passed into the `MapDisplayModelAsync` method. All you need to do is reference it from any queries you make to related entities that support versioning.

## Querying related data from non-Cofoundry code using IVisualEditorStateService

For non-Cofoundry code elements such as `ViewComponent` we have an injected service called `IVisualEditorStateService` which can be used to query the visual editor state and get the `PublishStatusQuery` to use for 'ambient' entities (i.e not the entity being edited).

In the example below we use the service to get an object representing the visual editor state for the request and then call `visualEditorState.GetAmbientEntityPublishStatusQuery()` to get a value we can use to query a blog post listing. If the visual editor is in *live* mode then only published blog posts will be returned from the query, otherwise the latest version (including drafts) will be returned.

```csharp
public class HomepageBlogPostsViewComponent : ViewComponent
{
    private readonly IVisualEditorStateService _visualEditorStateService;
    private readonly IContentRepository _contentRepository;

    public HomepageBlogPostsViewComponent(
        IContentRepository contentRepository,
        IVisualEditorStateService visualEditorStateService
        )
    {
        _visualEditorStateService = visualEditorStateService;
        _contentRepository = contentRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var visualEditorState = await _visualEditorStateService.GetCurrentAsync();

        var query = new SearchCustomEntityRenderSummariesQuery()
        {
            CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode,
            PublishStatus = visualEditorState.GetAmbientEntityPublishStatusQuery(),
            PageSize = 3
        };

        var entities = await _contentRepository
            .CustomEntities()
            .Search()
            .AsRenderSummaries(query)
            .ExecuteAsync();

        return View(entities);
    }
}
```

Note that `IVisualEditorStateService` is scoped to the ASP.NET request for the page and will not work from a WebApi request. 

