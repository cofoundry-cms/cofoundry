using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.DistributedLocks
{
    public interface IDistributedLockDefinition
    {
        string DistributedLockId { get; }

        string Name { get; }

        TimeSpan Timeout { get; }
    }
}
