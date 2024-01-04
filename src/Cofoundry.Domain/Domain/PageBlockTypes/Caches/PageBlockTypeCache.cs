using Cofoundry.Core.Caching;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageBlockTypeCache"/>.
/// </summary>
public class PageBlockTypeCache : IPageBlockTypeCache
{
    private const string SUMMARIES_CACHEKEY = "Summaries";
    private const string FILE_LOCATIONS_CACHEKEY = "FileLocations";
    private const string CACHEKEY = "Cofoundry.Domain.PageBlockTypeCache";

    private readonly IObjectCache _cache;

    public PageBlockTypeCache(IObjectCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Get(CACHEKEY);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<PageBlockTypeSummary>> GetOrAddAsync(Func<Task<IReadOnlyCollection<PageBlockTypeSummary>>> getter)
    {
        var result = await _cache.GetOrAddAsync(SUMMARIES_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAddAsync)} with key {SUMMARIES_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, PageBlockTypeFileLocation> GetOrAddFileLocations(Func<IReadOnlyDictionary<string, PageBlockTypeFileLocation>> getter)
    {
        var result = _cache.GetOrAdd(FILE_LOCATIONS_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAdd)} with key {FILE_LOCATIONS_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _cache.Clear();
    }
}
