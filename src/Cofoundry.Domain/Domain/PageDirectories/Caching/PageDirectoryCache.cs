using Cofoundry.Core.Caching;

namespace Cofoundry.Domain;

/// <summary>
/// Default implementation of <see cref="IPageDirectoryCache"/>.
/// </summary>
public class PageDirectoryCache : IPageDirectoryCache
{
    private const string PAGEDIRECTORYROUTES_CACHEKEY = "PageRoutes";
    private const string CACHEKEY = "COF_PageDirectories";

    private readonly IObjectCache _cache;
    private readonly IPageCache _pageCache;

    public PageDirectoryCache(
        IObjectCacheFactory cacheFactory,
        IPageCache pageCache
        )
    {
        _cache = cacheFactory.Get(CACHEKEY);
        _pageCache = pageCache;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<PageDirectoryRoute> GetOrAdd(Func<IReadOnlyCollection<PageDirectoryRoute>> getter)
    {
        var result = _cache.GetOrAdd(PAGEDIRECTORYROUTES_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAdd)} with key {PAGEDIRECTORYROUTES_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _cache.Clear();
        _pageCache.Clear();
    }
}
