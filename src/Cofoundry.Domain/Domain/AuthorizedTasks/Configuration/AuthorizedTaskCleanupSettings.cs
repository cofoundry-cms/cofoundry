using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain
{
    /// <summary>
    /// These settings control the background task that runs to clean up 
    /// completed, invalid or expired authorized tasks.
    /// </summary>
    public class AuthorizedTaskCleanupSettings : CofoundryConfigurationSettingsBase, IFeatureEnableable
    {
        /// <summary>
        /// If set to <see langword="false"/> the cleanup background task is disabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// How often the background task should run, measured in minutes. Defaults
        /// to 11 hours and 51 minutes.
        /// </summary>
        public int BackgroundTaskFrequencyInMinutes { get; set; } = 711;

        /// <summary>
        /// The time period to store data for completed, invalid or expired 
        /// tasks, measured in days. Defaults to 30 days. If zero, then data is
        /// removed as soon as the background task is run. If <see langword="null"/> or
        /// less than zero then task data is stored indefinitely.
        /// </summary>
        public int? RetentionPeriodInDays { get; set; } = 30;
    }
}