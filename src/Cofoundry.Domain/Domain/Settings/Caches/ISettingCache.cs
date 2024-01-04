namespace Cofoundry.Domain;

public interface ISettingCache
{
    void Clear();

    Task<IReadOnlyDictionary<string, string>> GetOrAddSettingsTableAsync(Func<Task<IReadOnlyDictionary<string, string>>> getter);
}
