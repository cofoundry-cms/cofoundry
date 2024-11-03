using Cofoundry.Core.Configuration;

namespace Cofoundry.Plugins.YouTube.Domain;

public class YouTubeSettings : PluginConfigurationSettingsBase
{
    public string? ApiKey { get; set; }
}
