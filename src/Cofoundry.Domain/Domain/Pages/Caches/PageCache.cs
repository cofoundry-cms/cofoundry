using Cofoundry.Core.Caching;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageCache"/>.
/// </summary>
public class PageCache : IPageCache
{
    private const string PAGEROUTES_CACHEKEY = "PageRoutes";
    private const string CACHEKEY = "COF_Pages";

    private readonly IObjectCache _cache;
    public PageCache(IObjectCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Get(CACHEKEY);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyDictionary<int, PageRoute>> GetOrAddAsync(Func<Task<IReadOnlyDictionary<int, PageRoute>>> getter)
    {
        var result = await _cache.GetOrAddAsync(PAGEROUTES_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAddAsync)} with key {PAGEROUTES_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <inheritdoc/>
    public void Clear(int pageId)
    {
        ClearRoutes();
    }

    /// <inheritdoc/>
    public void ClearRoutes()
    {
        _cache.Clear(PAGEROUTES_CACHEKEY);
    }
}