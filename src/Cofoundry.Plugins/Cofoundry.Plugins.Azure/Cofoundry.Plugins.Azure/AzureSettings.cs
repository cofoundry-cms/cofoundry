using Cofoundry.Core.Configuration;

namespace Cofoundry.Plugins.Azure;

public class AzureSettings : PluginConfigurationSettingsBase
{
    /// <summary>
    /// Indicates whether the plugin should be disabled, which means services
    /// will not be bootstrapped. Disable this in dev when you want to run using
    /// the standard non-cloud services. Defaults to false.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// The connection string to use when accessing files in blob storage
    /// </summary>
    public string? BlobStorageConnectionString { get; set; }
}
