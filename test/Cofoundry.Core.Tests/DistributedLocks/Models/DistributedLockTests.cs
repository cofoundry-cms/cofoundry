using Cofoundry.Core.DistributedLocks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class DistributedLockTests
    {
        #region IsLocked

        [Fact]
        public void IsLocked_WhenLocked_ReturnsTrue()
        {
            // Note dates should not matter as they are retrieved from 
            // the database as a snapshot of the state and the values are not  
            // relevant for the check, only that they exist or not exist
            var distributedLock = new DistributedLock()
            {
                DistributedLockId = "TSTTST",
                ExpiryDate = DateTime.MinValue,
                LockDate = DateTime.MinValue,
                LockedByLockingId = Guid.NewGuid()
            };

            Assert.True(distributedLock.IsLocked());
        }

        [Fact]
        public void IsLocked_WhenNotLocked_ReturnsFalse()
        {
            var distributedLock = new DistributedLock()
            {
                DistributedLockId = "TSTTST",
                ExpiryDate = DateTime.MinValue,
                LockDate = DateTime.MinValue
            };

            Assert.False(distributedLock.IsLocked());
        }

        #endregion

        #region IsLockedByAnotherProcess

        [Fact]
        public void IsLockedByAnotherProcess_WhenLockedBySelf_ReturnsFalse()
        {
            var lockingId = Guid.NewGuid();
            var distributedLock = new DistributedLock()
            {
                DistributedLockId = "TSTTST",
                ExpiryDate = DateTime.MinValue,
                LockDate = DateTime.MinValue,
                LockedByLockingId = lockingId,
                RequestedLockingId = lockingId
            };

            Assert.False(distributedLock.IsLockedByAnotherProcess());
        }

        [Fact]
        public void IsLockedByAnotherProcess_WhenNotLocked_ReturnsFalse()
        {
            var distributedLock = new DistributedLock()
            {
                DistributedLockId = "TSTTST"
            };

            Assert.False(distributedLock.IsLockedByAnotherProcess());
        }

        [Fact]
        public void IsLockedByAnotherProcess_WhenLockedByProcess_ReturnsTrue()
        {
            var distributedLock = new DistributedLock()
            {
                DistributedLockId = "TSTTST",
                ExpiryDate = DateTime.MinValue,
                LockDate = DateTime.MinValue,
                LockedByLockingId = Guid.NewGuid(),
                RequestedLockingId = Guid.NewGuid()
            };

            Assert.True(distributedLock.IsLockedByAnotherProcess());
        }

        #endregion
    }
}
