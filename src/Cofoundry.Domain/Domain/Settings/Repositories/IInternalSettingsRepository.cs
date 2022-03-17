namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used internally by other query handlers to get settings. Bypasses permissions
/// so should not be used outside of a query handler.
/// </summary>
public interface IInternalSettingsRepository
{
    Dictionary<string, string> GetAllSettings();
    Task<Dictionary<string, string>> GetAllSettingsAsync();
}
