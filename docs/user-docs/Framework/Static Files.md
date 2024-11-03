Cofoundry automatically sets up the static file middleware, serving files from the website content root (using `IHostingEnvironment.WebRootFileProvider`) as well as files embedded in Cofoundry and any plugin assemblies.

## IStaticFileViewHelper

The [Cofoundry view helper](/content-management/cofoundry-view-helper) includes a helper for for applying versioning to static files. This works in a similar way to the `asp-append-version` tag helper, the difference being that our version works across all registered static file providers, not just `IHostingEnvironment.WebRootFileProvider`.

```html
@using Cofoundry.Domain
@inject ICofoundryHelper Cofoundry

<div>
    <img src="@Cofoundry.StaticFiles.AppendVersion("/content/my-cat.png")">
    
    <span>@Cofoundry.StaticFiles.ToAbsoluteWithFileVersion("/content/my-cat.png")</span>
</div>

```

This service is also available by requesting `IStaticFileViewHelper` using the DI system.

## Configuring the static file middleware

Cofoundry sets up the static file middleware using a default configuration that simply sets caching headers using rules that should work for most configurations.

If you need to alter the configuration this can either be done using simple configuration settings, or by overriding the default `IStaticFileOptionsConfiguration` for more advanced scenarios.

### StaticFilesSettings

- **Cofoundry:StaticFiles:MaxAge** The default max-age to use for the cache control header. The default value is 1 year. General advice here for a maximum is 1 year.
- **Cofoundry:StaticFiles:CacheMode** The type of caching rule to use when adding caching headers. This defaults to StaticFileCacheMode.OnlyVersionedFiles which only sets caching headers for files using the "v" querystring parameter convention.

### Custom configuration

If you need more control over the static file configuration you can replace the Cofoundry `IStaticFileOptionsConfiguration` implementation using the [DI override system](dependency-injection#overriding-registrations). 

Example implementation:

```csharp
using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

public class SimpleStaticFileOptionsConfiguration : IStaticFileOptionsConfiguration
{
    private static readonly string[] CACHE_CONTEROL_VALUE = ["public,max-age=31536000"];

    public void Configure(StaticFileOptions options)
    {
        options.OnPrepareResponse = OnPrepareResponse;
    }

    private void OnPrepareResponse(StaticFileResponseContext context)
    {
        context.Context.Response.Headers.Append("Cache-Control", CACHE_CONTEROL_VALUE);
    }
}
```

## Serving static files embedded in assemblies

If you want to expose static files embedded in an assembly, this can be done by implementing `IEmbeddedResourceRouteRegistration` and registering the routes you want exposed:

```csharp
using Cofoundry.Core.ResourceFiles;

public class ExampleEmbeddedResourcetRouteRegistration : IEmbeddedResourceRouteRegistration
{
    public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
    {
        var path = new EmbeddedResourcePath(
            GetType().Assembly,
            "/MyAssemblyStaticFiles"
            );

        yield return path;
    }
}
```

Note that the URL for the files will mirror their location in the embedded assembly.

## Adding MIME type mappings to FileExtensionContentTypeProvider

In Cofoundry you can add additional MIME mappings to the ASP.NET Core  `FileExtensionContentTypeProvider` by creating a class that implements `IMimeTypeRegistration`:

```csharp
public class MyAdditionalMimeTypeRegistration : IMimeTypeRegistration
{
    public void Register(IMimeTypeRegistrationContext context)
    {
        context.AddOrUpdate(".epub", "application/epub+zip");
        context.AddOrUpdate(".csv", "text/csv");
    }
}
```