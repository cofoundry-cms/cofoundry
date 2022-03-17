using Cofoundry.Core.DistributedLocks;

namespace Cofoundry.Core.AutoUpdate;

/// <inheritdoc/>
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
