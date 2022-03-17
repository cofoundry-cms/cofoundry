namespace Cofoundry.Web;

/// <summary>
/// Helper for accessing configuration settings from a view
/// </summary>
public interface ISettingsViewHelper
{
    Task<TSettings> GetAsync<TSettings>() where TSettings : ICofoundrySettings;
}
