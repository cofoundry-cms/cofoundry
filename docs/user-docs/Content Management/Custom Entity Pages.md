A *Custom Entity Page* is a special *Page Type* that dynamically serves a page for every custom entity using a URL routing rule like *'{id}/{urlSlug}'*.

This is useful if you have a custom entity like blog posts or products where you want the advantages of having a dedicated custom entity type, but also have the advantages of the page templating and content block system. 

The page template can have special sections defined that allow content blocks to be inserted for each of the dynamically generated pages. Let's illustrate this with an example:

## Blog Posts Example

Firstly we need to create a display model that will be used in the template.

```csharp
using Cofoundry.Domain;
using Cofoundry.Web;

public class BlogPostDisplayModel : ICustomEntityPageDisplayModel<BlogPostDataModel>
{
    public string PageTitle { get; set; } = string.Empty;

    public string? MetaDescription { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string? Author { get; set; }
}
```

A mapper class is also required to tell Cofoundry how to transform the raw custom entity data to this display model. The mapper supports constructor injection so this also give you the opportunity to include any extra data you might need in the view.

```csharp
public class BlogPostDisplayModelMapper
    : ICustomEntityDisplayModelMapper<BlogPostDataModel, BlogPostDisplayModel>
{
    public Task<BlogPostDisplayModel> MapDisplayModelAsync(
        CustomEntityRenderDetails renderDetails,
        BlogPostDataModel dataModel,
        PublishStatusQuery publishStatusQuery
        )
    {
        var displayModel = new BlogPostDisplayModel
        {
            PageTitle = renderDetails.Title,
            MetaDescription = dataModel.ShortDescription,
            Title = renderDetails.Title,
            Date = renderDetails.CreateDate,
            Author = dataModel.Author
        };

        return Task.FromResult(displayModel);
    }
}
```


For the view we use an enhanced version of a *Page Template*, which should have a model type of `ICustomEntityPageViewModel<TDisplayModel>`. As with regular PageTemplates you'll need to inject `ICofoundryTemplatePageHelper<TModel>` in order to give access to the Cofoundry template helper and a strongly typed view model.

```html
@using Cofoundry.Domain
@using Cofoundry.Web

@model ICustomEntityPageViewModel<BlogPostDisplayModel>
@inject ICofoundryTemplateHelper<ICustomEntityPageViewModel<BlogPostDisplayModel>> Cofoundry


@{
    Cofoundry.Template.UseDescription("Template for the blog post details page");
    var post = Model.CustomEntity.Model;
}

<div class="blog-content">
    <h1>@post.Title</h1>
    <h2>@post.Date by @post.Author</h2>

    <div class="blog-lead">

        @(await Cofoundry.Template.CustomEntityRegion("Lead")
            .AllowBlockType<RichTextDataModel>()
            .EmptyContentMinHeight(500)
            .InvokeAsync())

    </div>

    <div class="blog-body">

        @(await Cofoundry.Template.CustomEntityRegion("Body")
            .AllowMultipleBlocks()
            .AllowBlockType<RawHtmlDataModel>()
            .EmptyContentMinHeight(500)
            .InvokeAsync())

    </div>
</div>
```

## Adding the Page

The template will be automatically registered with Cofoundry when the application is published or restarted. The page can then be added to the website in the *Pages* section of the admin panel as you would a regular page, except you will now be able to change the page type to be *Custom Entity Page*.

## Routing Rules

When adding the page you'll be able to select the dynamic routing rule used to locate the custom entity. Out of the box we support two routing rules:

- **`{UrlSlug}`** e.g. *"/blogs/my-first-blog"*
- **`{Id}/{UrlSlug}`** e.g. *"/blogs/1/my-first-blog"*
 
#### Creating a your own routing rules

Routing rules are created by implementing `ICustomEntityRoutingRule`. The built-in routing rules are created using this, so you can check the code to see how they work:

- [`UrlSlugCustomEntityRoutingRule`](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Domain/Domain/CustomEntities/Models/RoutingRules/UrlSlugCustomEntityRoutingRule.cs)
- [`IdAndUrlSlugCustomEntityRoutingRule`](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Domain/Domain/CustomEntities/Models/RoutingRules/IdAndUrlSlugCustomEntityRoutingRule.cs)

New routing rules are automatically picked up by the DI system.

### Additional Route Data

The `CustomEntityRoute` object used in `ICustomEntityRoutingRule` gives you access to basic properties like `CustomEntityId` or `UrlSlug`, but what if you need to use more specific data such as a related entity id or data property? 

To this we can take advantage of the `CustomEntityRoute.AdditionalRoutingData` collection. We can add values to this in one of two ways:

#### CustomEntityRouteDataAttribute

To include properties from the custom entity data model, you can annotate a property with `[CustomEntityRouteData]`. In the example below, the value for `BlogCategoryId` will be included in the `AdditionalRoutingData` collection using property name *BlogCategoryId* as the key:

```csharp
public class BlogPostDataModel : ICustomEntityDataModel
{
    [MaxLength(1000)]
    [Required]
    [MultiLineText]
    public string ShortDescription { get; set; } = string.Empty;

    [CustomEntityRouteData]
    [CustomEntity(BlogCategoryDefinition.DefinitionCode)]
    [PositiveInteger]
    [Required]
    public int BlogCategoryId { get; set; }
}
```

#### ICustomEntityRouteDataBuilder

For full control over the data that gets added to the `AdditionalRoutingData` collection, you can create a class that implements `ICustomEntityRouteDataBuilder`. These classes run when batches of `CustomEntityRoute` objects are materialized, but before they are added to the cache, allowing you to augment the `AdditionalRoutingData` collection as you please.

Note that custom entity routes are created and cached in large sets to take advantage of batch operations, so be careful running looped or extensive queries in your builder.

```csharp
public class BlogPostRouteDataBuilder : ICustomEntityRouteDataBuilder<BlogPostCustomEntityDefinition, BlogPostDataModel>
{
    private readonly IContentRepository _contentRepository;

    public BlogPostRouteDataBuilder(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task BuildAsync(IReadOnlyCollection<CustomEntityRouteDataBuilderParameter<BlogPostDataModel>> builderParameters)
    {
        // get all categories to use in the mapping
        var categories = await _contentRepository
            .CustomEntities()
            .GetByDefinition<BlogCategoryCustomEntityDefinition>()
            .AsRenderSummaries()
            .ExecuteAsync();

        // index the categories to speed up the lookup
        var categoriesLookup = categories.ToDictionary(e => e.CustomEntityId);

        // each builderParameter represents the route for a blog post version
        foreach (var builderParameter in builderParameters)
        {
            // find the category and add it to the routing data
            var category = categoriesLookup.GetValueOrDefault(builderParameter.DataModel.BlogCategoryId);
            if (category != null)
            {
                builderParameter.AdditionalRoutingData.Add("CategoryUrlSlug", category.UrlSlug);
            }
        }
    }
}
```