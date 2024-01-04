using Cofoundry.Core.Caching;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="ILocaleCache"/>.
/// </summary>
public class LocaleCache : ILocaleCache
{
    private const string CACHEKEY = "COF_Locales";
    private const string ACTIVELOCALE_CACHEKEY = "ActiveLocales";

    private readonly IObjectCache _cache;
    public LocaleCache(IObjectCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Get(CACHEKEY);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<ActiveLocale>> GetOrAddAsync(Func<Task<IReadOnlyCollection<ActiveLocale>>> getter)
    {
        var result = await _cache.GetOrAddAsync(ACTIVELOCALE_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAddAsync)} with key {ACTIVELOCALE_CACHEKEY} should never be null.");
        }

        return result;
    }
}
