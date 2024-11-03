*Search engine optimization* is an important consideration for most public sites. There are a number of elements that make up a good SEO strategy which will largely be driven by non-technical considerations such as content and link building.

For the technical side of SEO, Cofoundry has a number of tools you might find useful, however since Cofoundry is unobtrusive by nature, it's up to you to use these tools in a way that fits your SEO needs.

The [Ahrefs blog](https://ahrefs.com/blog/) is a great resource for up-to-date SEO information.

## Metadata

SEO advice for meta data changes over time and often depends on the sort of content you are serving. Cofoundry provides the raw metadata properties for dynamic content, but it's up to you to render them in a way that suits your content and SEO needs.

[Pages](pages) represent dynamically generated page content and include properties for meta and open graph data which can be edited in the admin panel. 

This data can be accessed in a number of ways:

### Metadata in models

If you're working with page data models directly, you'll find the meta data and open graph data included in both the `PageRenderSummary` and `PageRenderDetails` models.

```csharp
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cofoundry.Domain;

public class ExampleController : Controller
{
    private readonly IContentRepository _contentRepository;

    public ExampleController(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [Route("example/page/{id:int}")]
    public async Task<IActionResult> Page(int id)
    {
        var page = await _contentRepository
            .Pages()
            .GetById(id)
            .AsRenderDetails()
            .ExecuteAsync();

        return Json(new
        {
            title = page.Title,
            metaDescription = page.MetaDescription,
            ogTitle = page.OpenGraph.Title,
            ogDescription = page.OpenGraph.Description,
            ogImage = page.OpenGraph.Image
        });
    }
}
```

### Metadata in page templates

Inside page templates you can access metadata through the `Model.Page` property:

```html
@model IPageViewModel
@inject ICofoundryTemplateHelper<IPageViewModel> Cofoundry

<h3>Open graph Description</h3>
<p>
    @Model.Page.OpenGraph.Description
</p>
<h3>Open graph image</h3>
<img src="@Cofoundry.Routing.ImageAsset(Model.Page.OpenGraph.Image)" title="@Model.Page.OpenGraph.Image.Title" />

```

### Accessing metadata in layout files

Typically you'll want to deal with metadata in your layout file, or at least provide sensible defaults which can be overridden in your template or view files.

There's a few ways to handle this, but it's important to remember that we're using standard Razor templates and anything you'd normally do in ASP.NET views will work here.

#### Casting

All pages served by Cofoundry inherit from `IPageWithMetaDataViewModel`, this includes not just dynamic pages, but also 404 and error pages. In your layout page you can cast to this model to access basic metadata.

You could even cast to a more specific model such as `IPageViewModel` but be aware that status code and error pages do not inherit from `IPageViewModel` so it's important to check the cast will work first.

```html
@inject ICofoundryHelper Cofoundry
@{
    Layout = null;

    var title = "Example site";
    var description = "Welcome to the example site";

    if (Model is IPageWithMetaDataViewModel)
    {
        var metaDataModel = (IPageWithMetaDataViewModel)Model;

        title = StringHelper.FirstNotNullOrWhitespace(metaDataModel.PageTitle, title);
        description = StringHelper.FirstNotNullOrWhitespace(metaDataModel.MetaDescription, description);
    }
}
<!DOCTYPE html>
<html lang="en-us">
<head>
    <meta charset="utf-8">
    
    <title>@title</title>
    <meta name="description" content="@description">
</head>
<body>
    ...
</body>
</html>
```

#### Using ViewBag

You can also pass data to your layout file via `ViewBag`:

```html
@inject ICofoundryHelper Cofoundry
@{
    Layout = null;

    var canonicalUrl = ViewBag.CanonicalUrl;
}
<!DOCTYPE html>
<html lang="en-us">
<head>
    <meta charset="utf-8">
    
    @if (!string.IsNullOrEmpty(canonicalUrl))
    {
        var url = Cofoundry.Routing.ToAbsolute(canonicalUrl);
        <meta property="og:url" content="@Html.Raw(url)" />
        <link rel="canonical" href="@Html.Raw(url)" />
    }
</head>
<body>
    ...
</body>
</html>
```

#### Conditional sections

Another strategy is to allow view/templates to define their own metadata sections to either enrich or override the defaults.

```html
@inject ICofoundryHelper Cofoundry
@{
    Layout = null;
}
<!DOCTYPE html>
<html lang="en-us">
<head>
    <meta charset="utf-8">
    
    @if (IsSectionDefined("OpenGraph"))
    {
        @RenderSection("OpenGraph")
    }
    else
    {
        <meta property="og:title" content="Example Site" />
        <meta property="og:description" content="Welcome to the example site" />
        <meta property="og:type" content="website" />
    }
    @RenderSection("Meta", false)
</head>
<body>
    ...
</body>
</html>
```

## Resolving absolute URLs for meta tags

For the canonical URL and some meta tags such as open graph URLs and images you'll need to resolve an absolute URL.

You can use the [Cofoundry routing helper](routing#routing-view-helper) to do this:

```html
@using Cofoundry.Domain

@inject ICofoundryHelper Cofoundry

@{
    var url = Cofoundry.Routing.ToAbsolute("/my-relative-url");
}

<meta property="og:url" content="@Html.Raw(url)" />
<link rel="canonical" href="@Html.Raw(url)" />
```

See the docs for the [Routing view helper](routing#routing-view-helper) for more detailed information.

## SiteMap

The [Cofoundry sitemap plugin](https://github.com/cofoundry-cms/Cofoundry.Plugins.SiteMap) is a quick, easy and extensible way to add a sitemap to your site.

## Rewrite rules

See the [Rewrite Rules section](rewrite-rules) for details 