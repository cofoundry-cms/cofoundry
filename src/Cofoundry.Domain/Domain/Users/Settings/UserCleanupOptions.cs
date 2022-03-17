using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain;

/// <summary>
/// These settings control the background task that runs to clean up 
/// stale user data.
/// </summary>
public class UserCleanupOptions : IFeatureEnableable
{
    /// <summary>
    /// If set to <see langword="false"/> the cleanup background task is disabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// How often the background task should run, measured in minutes. Defaults
    /// to 11 hours and 27 minutes. Note that the background task processed all
    /// user areas and so customizing this setting for a specific user area has 
    /// no effect.
    /// </summary>
    [Range(1, 1439)]
    public int BackgroundTaskFrequencyInMinutes { get; set; } = 687;

    /// <summary>
    /// The default retention period for stale data, measured in days. If zero, then data is
    /// removed as soon as the background task is run. If <see langword="null"/> or
    /// less than zero then task data is stored indefinitely.
    /// </summary>
    public int? DefaultRetentionPeriodInDays { get; set; } = 30;

    /// <summary>
    /// The amount of time to keep records in the <see cref="UserAuthenticationLog"/> table, measured in days. 
    /// If <see langword="null"/> then the value defaults to the <see cref="DefaultRetentionPeriod"/>. If set 
    /// to less than zero then task data is stored indefinitely.
    /// </summary>
    public int? AuthenticationLogRetentionPeriodInDays { get; set; }

    /// <summary>
    /// The amount of time to keep records in the <see cref="UserAuthenticationFailLog"/> table, measured in days. 
    /// If <see langword="null"/> then the value defaults to the <see cref="DefaultRetentionPeriod"/>. If set 
    /// to less than zero then task data is stored indefinitely.
    /// </summary>
    public int? AuthenticationFailLogRetentionPeriodInDays { get; set; }

    /// <summary>
    /// Copies the options to a new instance, which can be modified
    /// without altering the base settings. This is used for user area
    /// specific configuration.
    /// </summary>
    public UserCleanupOptions Clone()
    {
        return new UserCleanupOptions()
        {
            Enabled = Enabled,
            BackgroundTaskFrequencyInMinutes = BackgroundTaskFrequencyInMinutes,
            DefaultRetentionPeriodInDays = DefaultRetentionPeriodInDays,
            AuthenticationLogRetentionPeriodInDays = AuthenticationLogRetentionPeriodInDays,
            AuthenticationFailLogRetentionPeriodInDays = AuthenticationFailLogRetentionPeriodInDays
        };
    }
}
