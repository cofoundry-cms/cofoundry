using Cofoundry.Core.DistributedLocks;

namespace Cofoundry.Domain;

public class CleanUpAssetFilesCommandDistributedLockDefinition : IDistributedLockDefinition
{
    public string DistributedLockId => "COFCUA";

    public string Name => "Asset file cleanup process lock";

    public TimeSpan Timeout => TimeSpan.FromMinutes(30);
}
