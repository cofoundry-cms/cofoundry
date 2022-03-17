using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.ExecutionDurationRandomizer;

/// <summary>
/// System-wide configuration settings for the randomized
/// execution duration system.
/// </summary>
public class ExecutionDurationRandomizerSettings : CofoundryConfigurationSettingsBase, IFeatureEnableable
{
    /// <summary>
    /// If set to <see langword="false"/> all usage of randomized execution
    /// duration features are ignored.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
