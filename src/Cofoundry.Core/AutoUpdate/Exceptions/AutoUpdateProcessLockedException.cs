using Cofoundry.Core.DistributedLocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// An exception that is thrown when the auto-update process
    /// attempts to run, but cannot because the process is locked by 
    /// another instance that is already running the update process.
    /// Typically this should only occur in a web-farm deployment 
    /// scenario whereby multiple machines startup at once.
    /// </summary>
    public class AutoUpdateProcessLockedException : Exception
    {
        public AutoUpdateProcessLockedException(DistributedLock distributedLock)
            : base(FormatMessage(distributedLock))
        {
            DistributedLock = distributedLock;
        }

        public DistributedLock DistributedLock { get; set; }

        private static string FormatMessage(DistributedLock distributedLock)
        {
            return $"The auto-update process cannot be started because it has been locked by another " +
                $"machine on {distributedLock.LockDate}. The lock is due to expire on {distributedLock.ExpiryDate}.";
        }
    }
}
