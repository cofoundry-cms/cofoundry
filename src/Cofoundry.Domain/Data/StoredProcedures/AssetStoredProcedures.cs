using Cofoundry.Core.EntityFramework;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Data access abstraction over stored procedures for image and document asset entities.
    /// </summary>
    public class AssetStoredProcedures : IAssetStoredProcedures
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly CofoundryDbContext _dbContext;

        public AssetStoredProcedures(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            CofoundryDbContext dbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _dbContext = dbContext;
        }

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
        public Task CleanupAssetFileCleanupQueueItemAsync(double completedItemRetentionTimeInSeconds, double deadLetterRetentionTimeInSeconds)
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.AssetFileCleanupQueueItem_Cleanup",
                 new SqlParameter("@CompletedItemRetentionTimeInSeconds", completedItemRetentionTimeInSeconds),
                 new SqlParameter("@deadLetterRetentionTimeInSeconds", deadLetterRetentionTimeInSeconds)
                 );
        }
    }
}
