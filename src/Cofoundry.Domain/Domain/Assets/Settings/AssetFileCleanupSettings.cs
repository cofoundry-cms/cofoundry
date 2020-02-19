using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// These settings control the background task that runs to clean up 
    /// deleted asset files. Asset files are deleted as a background process 
    /// to avoid file locking issues.
    /// </summary>
    public class AssetFileCleanupSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// If set to true the cleanup background task is disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// How often the background task should run, measured in minutes.
        /// </summary>
        public int BackgroundTaskFrequencyInMinutes { get; set; } = 60;

        /// <summary>
        /// The number of queue items (files) to process each time the
        /// background task runs.
        /// </summary>
        public int BatchSize { get; set; } = 60;

        /// <summary>
        /// The number of minutes to store data on completed items in
        /// the queue.
        /// </summary>
        public int CompletedItemRetentionTimeInMinutes { get; set; } = 43200;

        /// <summary>
        /// The number of minutes to stored data on unprocessable items in
        /// the queue.
        /// </summary>
        public int DeadLetterRetentionTimeInMinutes { get; set; } = 525600;

        /// <summary>
        /// The total number of days to keep attempting to clean up
        /// an item in the queue.
        /// </summary>
        public int MaxRetryWindowInDays { get; set; } = 20;

        /// <summary>
        /// The initial time to wait before trying again to clean up
        /// an item that previously errored. This is multiplied by
        /// the RetryOffsetMultiplier on each failed attempt.
        /// </summary>
        public int RetryOffsetInMinutes { get; set; } = 20;

        /// <summary>
        /// The multiplier to use when incrementing the next attempt
        /// date when an item in the queue fails to be processed.
        /// </summary>
        public int RetryOffsetMultiplier { get; set; } = 3;
    }
}
