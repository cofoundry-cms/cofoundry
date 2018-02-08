using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Represents the state of the distributed lock record for the
    /// auto-update process.
    /// </summary>
    public class AutoUpdateDistributedLock
    {
        public string DistributedLockId { get; set; }

        public Guid LockingId { get; set; }

        public DateTime? LockDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
