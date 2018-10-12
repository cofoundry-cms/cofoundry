using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.DistributedLocks
{
    /// <summary>
    /// Represents the state of the distributed lock at the point at
    /// which a new locking request has been made.
    /// </summary>
    public class DistributedLock
    {
        public string DistributedLockId { get; set; }

        /// <summary>
        /// The locking id that was used to attempt the lock.
        /// </summary>
        public Guid RequestedLockingId { get; set; }

        /// <summary>
        /// The unique locking id of the process that created the lock.
        /// </summary>
        public Guid? LockedByLockingId { get; set; }

        /// <summary>
        /// The date the lock was created.
        /// </summary>
        public DateTime? LockDate { get; set; }

        /// <summary>
        /// The date the lock is set to expire.
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        public bool IsLocked()
        {
            // We shouldn't check expiry date here as this will
            // have been detected when record was fetched from the db 
            // and may not be up to date.
            return LockedByLockingId.HasValue &&  LockDate.HasValue;
        }

        public bool IsLockedByAnotherProcess()
        {
            return IsLocked() && RequestedLockingId != LockedByLockingId;
        }
    }
}
