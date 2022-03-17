using Cofoundry.Core.Configuration;

namespace Cofoundry.Web;

/// <summary>
/// Settings for managed content (e.g. pages/files/custom entities)
/// </summary>
public class ContentSettings : CofoundryConfigurationSettingsBase
{
    /// <summary>
    /// A developer setting which can be used to view unpublished versions of content 
    /// without being signed into the administrator site.
    /// </summary>
    public bool AlwaysShowUnpublishedData { get; set; }
}
