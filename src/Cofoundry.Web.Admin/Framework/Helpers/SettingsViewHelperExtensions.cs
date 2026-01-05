namespace Cofoundry.Web.Admin;

/// <summary>
/// Extension methods for <see cref="ISettingsViewHelper"/>.
/// </summary>
public static class SettingsViewHelperExtensions
{
    extension(ISettingsViewHelper helper)
    {
        public async Task<string?> GetApplicationNameAsync()
        {
            var settings = await helper.GetAsync<GeneralSiteSettings>();
            return settings.ApplicationName;
        }
    }
}
