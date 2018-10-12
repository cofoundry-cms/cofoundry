using Cofoundry.Core.DistributedLocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Used to define the identity for a distributed lock.
    /// </summary>
    public class AutoUpdateDistributedLockDefinition : IDistributedLockDefinition
    {
        public AutoUpdateDistributedLockDefinition(
            AutoUpdateSettings autoUpdateSettings
            )
        {
            Timeout = TimeSpan.FromSeconds(autoUpdateSettings.ProcessLockTimeoutInSeconds);
        }

        public string DistributedLockId { get; } = "COFUPD";

        public string Name { get; } = "Cofoundry auto-update process";

        public TimeSpan Timeout { get; private set; }
    }
}
