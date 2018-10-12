using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Removes old completed entries from the AssetFileCleanupQueueItem
    /// table.
    /// </summary>
    public class CleanupAssetFileCleanupQueueCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The amount of time to keep completed items in the queue.
        /// </summary>
        public TimeSpan CompletedItemRetentionTime { get; set; } = TimeSpan.FromDays(90);

        /// <summary>
        /// The amount of time to keep items that have failed too many time and
        /// have the "CanRetry" flag set to false.
        /// </summary>
        public TimeSpan DeadLetterRetentionTime { get; set; } = TimeSpan.FromDays(365);
    }
}
