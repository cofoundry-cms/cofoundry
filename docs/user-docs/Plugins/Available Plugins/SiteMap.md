The SiteMap plugin is a quick and easy way to add a dynamic sitemap to your site. 

> Sitemaps are an easy way for webmasters to inform search engines about pages on their sites that are available for crawling. In its simplest form, a Sitemap is an XML file that lists URLs for a site along with additional metadata about each URL
>
> &mdash; [sitemaps.org](https://www.sitemaps.org/)

## Installation

Install the [Cofoundry.Plugins.SiteMap](https://www.nuget.org/packages/Cofoundry.Plugins.SiteMap/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.SiteMap
```

## Usage

All pages and custom entities routes are added to the sitemap automatically and additional pages can be added easily using an `ISiteMapResourceRegistration` class.

The sitemap is located at **/sitemap.xml**

### Custom Site Map Pages

If you have standard MVC routes or have a SPA app that handles routing on the client side you'll have to add these routes to the sitemap manually. You can add items to the sitemap by creating a class that implements `ISiteMapResourceRegistration` or `IAsyncSiteMapResourceRegistration`, which will automatically be picked up and registered with the sitemap:

```csharp
using Cofoundry.Plugins.SiteMap;

public class SiteMapRegistration : ISiteMapResourceRegistration
{
    public IEnumerable<ISiteMapResource> GetResources()
    {
        yield return new SiteMapResource("/");
        yield return new SiteMapResource("/blog");
        yield return new SiteMapResource("/about");
    }
}

```

If you want to dynamically generate a collection of links from a data source asynchronously you can instead implement `IAsyncSiteMapResourceRegistration`. Constructor dependency injection is supported.

### Customizing The Site Map XML File

For more control over how the sitemap xml file is rendered you should override the default `ISiteMapBuilderFactory` implementation ([override via DI](https://www.cofoundry.org/docs/framework/dependency-injection#overriding-registrations)) to return a custom `ISiteMapBuilder`, which will give you full control of the output.
