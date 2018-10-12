using Cofoundry.Core.DistributedLocks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Manages locking of the auto-update process to prevent concurrent update
    /// processes being run in multi-instance deployments.
    /// </summary>
    public interface IAutoUpdateDistributedLockManager
    {
        /// <summary>
        /// Locks the auto-update process using a unique
        /// identifier.
        /// </summary>
        Task<DistributedLock> LockAsync();

        /// <summary>
        /// Unlocks the auto-update process, indicating that the process
        /// has finished (or failed) and can be run again.
        /// </summary>
        /// <param name="lockingId">
        /// Unique identifier that represent the process that owns the lock.
        /// </param>
        Task UnlockAsync(DistributedLock distributedLock);
    }
}
