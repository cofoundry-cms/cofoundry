using Cofoundry.Core.Caching;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="ICustomEntityCache"/>.
/// </summary>
public class CustomEntityCache : ICustomEntityCache
{
    private const string ROUTES_CACHEKEY = "Routes_";
    private const string CACHEKEY = "COF_CustomEntities";

    private readonly IObjectCache _cache;
    public CustomEntityCache(IObjectCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Get(CACHEKEY);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<CustomEntityRoute>> GetOrAddAsync(string customEntityTypeCode, Func<Task<IReadOnlyCollection<CustomEntityRoute>>> getter)
    {
        var cacheKey = GetEntityTypeRoutesCacheKey(customEntityTypeCode);
        var result = await _cache.GetOrAddAsync(cacheKey, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAddAsync)} with key {cacheKey} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <inheritdoc/>
    public void Clear(string customEntityTypeCode, int customEntityId)
    {
        ClearRoutes(customEntityTypeCode);
    }

    /// <inheritdoc/>
    public void ClearRoutes(string customEntityTypeCode)
    {
        _cache.Clear(GetEntityTypeRoutesCacheKey(customEntityTypeCode));
    }

    private string GetEntityTypeRoutesCacheKey(string customEntityTypeCode)
    {
        return ROUTES_CACHEKEY + customEntityTypeCode;
    }
}
