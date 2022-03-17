using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain;

/// <summary>
/// Settings for configuring the Cofoundry admin portal.
/// </summary>
public class AdminSettings : CofoundryConfigurationSettingsBase
{
    public AdminSettings()
    {
        DirectoryName = "Admin";
    }

    /// <summary>
    /// Disables the admin panel, removing all routes from
    /// the routing table and disabling sign in.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// The path to the admin panel. Defaults to "admin". Can only
    /// contain letters, numbers and dashes.
    /// </summary>
    [Required]
    [Slug]
    public string DirectoryName
    {
        get { return _directoryName; }
        set { _directoryName = value?.ToLowerInvariant(); }
    }
    private string _directoryName = null;

    /// <summary>
    /// Indicates whether to automatically inject the visual editor
    /// into your content managed pages and other MVC results. 
    /// Enabled by default.
    /// </summary>
    public bool AutoInjectVisualEditor { get; set; } = true;
}
