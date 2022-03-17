using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used internally by other query handlers to get settings. Bypasses permissions
/// so should not be used outside of a query handler.
/// </summary>
public class InternalSettingsRepository : IInternalSettingsRepository
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ISettingCache _settingsCache;

    public InternalSettingsRepository(
        CofoundryDbContext dbContext,
        ISettingCache settingsCache
        )
    {
        _dbContext = dbContext;
        _settingsCache = settingsCache;
    }

    public async Task<Dictionary<string, string>> GetAllSettingsAsync()
    {
        var settings = await _settingsCache.GetOrAddSettingsTableAsync(() =>
        {
            return _dbContext
                .Settings
                .AsNoTracking()
                .ToDictionaryAsync(k => k.SettingKey, v => v.SettingValue);
        });

        return settings;
    }

    public Dictionary<string, string> GetAllSettings()
    {
        var settings = _settingsCache.GetOrAddSettingsTable(() =>
        {
            return _dbContext
                .Settings
                .AsNoTracking()
                .ToDictionary(k => k.SettingKey, v => v.SettingValue);
        });

        return settings;
    }
}