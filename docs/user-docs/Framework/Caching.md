Cofoundry has an object caching abstraction that it uses internally to store frequently accessed data. Currently the only supported implementation is our in-memory cache.

## Supporting Multi-Server Deployments

By default the in-memory cache is designed to run in a single-server environment. If you need to scale out to a multi-server deployment, you'll either need to reconfigure the in-memory cache or create your own distributed cache implementation.

#### Configuring the in-memory cache

A quick and simple way to support multi-server deployments is to change the lifetime of the cache to be per scope/request. This will reduce the performance of the cache, but sometimes a distributed cache service is simply not available and it's the only option.

The `CacheMode` can be changed to `PerScope` in your settings file:

```json
{
  "Cofoundry": {
    "InMemoryObjectCache:CacheMode": "PerScope"
  }
}
```

#### Implementing a distributed cache

Currently there are no distributed cache plugin packages, but it should be straightforward to build your own by implementing [`IObjectCacheFactory`](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Core/Caching/IObjectCacheFactory.cs) and overriding the default implementation using [our DI system](/framework/dependency-injection#overriding-registrations). Take a [look at our implementation](https://github.com/cofoundry-cms/cofoundry/tree/master/src/Cofoundry.Core/Caching/InMemoryObjectCache) to see how it works.

We hope to add additional distributed caching options soon - see [Issue #46](https://github.com/cofoundry-cms/cofoundry/issues/46).

## Using IObjectCacheFactory

You can use our caching framework by requesting `IObjectCacheFactory` from the DI container. `IObjectCacheFactory` will manage the cache state, so all we need to is request a cache store using a unique name:

```csharp
using Cofoundry.Core.Caching;

public class CacheSample
{
    public CacheSample(IObjectCacheFactory cacheFactory)
    {
        var myCache = cacheFactory.Get("MyExampleCache");
    }
}
```

## Creating Modular Caches

Internally we modularize our caches by creating one for each entity type we're caching. This pattern is optional, but you might find it useful if you're using the cache in many places:

```csharp
public class ImageAssetCache
{
    private const string IMAGE_ASSET_RENDER_DETAILS_CACHEKEY = "ImageAssetRenderDetails:";
    private readonly IObjectCache _cache;

    public ImageAssetCache(
        IObjectCacheFactory cacheFactory
        )
    {
        _cache = cacheFactory.Get("COF_ImageAssets");
    }

    public async Task<ImageAssetRenderDetails?> GetOrAddAsync(int imageAssetId, Func<Task<ImageAssetRenderDetails?>> getter)
    {
        var cacheKey = IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId;
        var result = await _cache.GetOrAddAsync(cacheKey, getter);

        return result;
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void Clear(int imageAssetId)
    {
        _cache.Clear(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
    }
}
```
