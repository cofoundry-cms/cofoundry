using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Data access abstraction over stored procedures for image and document asset entities.
    /// </summary>
    public interface IAssetStoredProcedures
    {
        /// <summary>
        /// Removes old completed entries from the AssetFileCleanupQueueItem
        /// table.
        /// </summary>
        /// <param name="completedItemRetentionTimeInSeconds">
        /// The amount of tiem in seconds to keep completed items in the queue.
        /// </param>
        /// <param name="deadLetterRetentionTimeInSeconds">
        /// The amount of time in seconds to keep items that have failed too many 
        /// times andhave the "CanRetry" flag set to false.
        /// </param>
        /// <returns></returns>
        Task CleanupAssetFileCleanupQueueItemAsync(double completedItemRetentionTimeInSeconds, double deadLetterRetentionTimeInSeconds);
    }
}
