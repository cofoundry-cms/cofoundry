using Cofoundry.Core.Caching;

namespace Cofoundry.Domain.Internal;

public class SettingCache : ISettingCache
{
    private const string CACHEKEY = "COF_Settings";
    private const string SETTING_TABLE_KEY = "SettingTable";

    private readonly IObjectCache _cache;

    public SettingCache(IObjectCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Get(CACHEKEY);
    }

    public async Task<IReadOnlyDictionary<string, string>> GetOrAddSettingsTableAsync(Func<Task<IReadOnlyDictionary<string, string>>> getter)
    {
        var result = await _cache.GetOrAddAsync(SETTING_TABLE_KEY, getter);
        return result ?? ImmutableDictionary<string, string>.Empty;
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
