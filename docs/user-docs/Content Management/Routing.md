## Dynamic Page Routing

Cofoundry has content management features that let you create and manage your website pages dynamically using templates.

For more information about dynamic page routing see the [Pages documentation](Pages)

## Rule Base Routing

Cofoundry also supports traditional rule based routing so you can use regular MVC controllers and actions as normal, however to support dynamic pages Cofoundry has to control the registration of routes. To register your rule based routes you should implement an `IRouteRegistration` class, which will automatically be picked up by the Dependency Injector system and injected into the Cofoundry route registration at the right time.

#### Example IRouteRegistration

Here is an example `IRouteRegistration` class:

```csharp
public class RouteRegistration : IRouteRegistration
{
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapControllerRoute(
            "Default", 
            "{controller}/{action}/{id?}",
            new { controller = "Home", action = "Index" }
        );
    }
}
```

You can have multiple registration classes (in advanced scenarios you may wish to modularize your application).

#### Ordering route registration

For most scenarios you won't need to worry about the ordering of your route registration classes, but in some advanced scenarios you may wish to take advantage of these additional interfaces which help you control the ordering that route registrations apply:

- **`IOrderedRouteRegistration`:** Implement this interface to define a custom ordering value for your route registration. Although this value is an integer, the `RouteRegistrationOrdering` enum defines some predefined values which you can use.
- **`IRunBeforeRouteRegistration`:** This interface is used to indicate that the registration should be run before one or more other registrations (indicated by type) are executed.
- **`IRunAfterRouteRegistration`:** This interface is used to indicate that the registration should be run after one or more other registrations (indicated by type) are executed.

## Attribute Routing

ASP.NET Core attribute routing is supported so if you prefer to use attribute routing then just use that as you normally would.

## Working with URLs

### Urls as properties

Some Cofoundry entity models contain url properties:

```csharp
public void LocateUrls(
    PageRoute pageRoute,
    PageRenderDetails pageRenderDetails,
    CustomEntityRenderSummary customEntityRenderSummary,
    ImageAssetRenderDetails imageAssetRenderDetails
    )
{
    string? url;
            
    url = pageRoute.FullUrlPath;
    url = pageRenderDetails.PageRoute.FullUrlPath;
    url = customEntityRenderSummary.PageUrls.FirstOrDefault();
    url = imageAssetRenderDetails.Url;
}
```

### IContentRouteLibrary

Sometime you need to construct a URL, such as for a resized image asset or a downloadable document. `IContentRouteLibrary` is an injectable helper that you can use to do this and can also be used as a consistent way of retrieving URLs for entities without having to dig around and find the right property.

```csharp
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;

public class ExampleController : Controller
{
    private readonly IContentRouteLibrary _contentRouteLibrary;
    private readonly IContentRepository _contentRepository;

    public ExampleController(
        IContentRouteLibrary contentRouteLibrary,
        IContentRepository contentRepository
        )
    {
        _contentRouteLibrary = contentRouteLibrary;
        _contentRepository = contentRepository;
    }

    [Route("example/page/{id:int}")]
    public async Task<IActionResult> Page(int id)
    {
        var page = await _contentRepository
            .Pages()
            .GetById(id)
            .AsRoute()
            .ExecuteAsync();
        var url = _contentRouteLibrary.Page(page);

        return Json(new
        {
            page,
            url
        });
    }

    [Route("example/doc/{id:int}")]
    public async Task<IActionResult> Document(int id)
    {
        var document = await _contentRepository
            .DocumentAssets()
            .GetById(id)
            .AsRenderDetails()
            .ExecuteAsync();
        var url = _contentRouteLibrary.DocumentAssetDownload(document);
        var absoluteUrl = _contentRouteLibrary.ToAbsolute(url);

        return Json(new
        {
            document,
            url,
            absoluteUrl
        });
    }
}
```

### Routing view helper

By referencing the [Cofoundry view helper](cofoundry-view-helper) in your view you will be able to conveniently access `IContentRouteLibrary` via `@Cofoundry.Routing`.

For example:

```html
@using Cofoundry.Domain

@model MyTestViewModel
@inject ICofoundryHelper<MyTestViewModel> Cofoundry


<div>
    <img src="@Cofoundry.Routing.ImageAsset(Model.HeaderImageAsset, 400, 300)">
    
    <a href="@Cofoundry.Routing.Page(Model.ExamplePage)">@Model.ExamplePage.Title</a>
</div>
```

### Resolving absolute URLs

All standard routing methods and properties are relative urls, but sometimes you'll want to convert them to be absolute.

A good example of this is with the canonical meta tag:

```html
@{
    var url = Cofoundry.Routing.ToAbsolute("/my-relative-url");
}

<meta property="og:url" content="@Html.Raw(url)" />
<link rel="canonical" href="@Html.Raw(url)" />
```

By default `Cofoundry.Routing.ToAbsolute("/my-relative-url")` will use the host and scheme of the incoming request to create the url, but it's a better idea to set this manually in your config using the `Cofoundry:SiteUrlResolver:SiteUrlRoot` setting to make sure it's always the same, even if you have multiple domains mapped to the same resource.

This can be set in config:

```json
{
  "Cofoundry": {
    "SiteUrlResolver:SiteUrlRoot": "https://www.cofoundry.org"
  }
}
```