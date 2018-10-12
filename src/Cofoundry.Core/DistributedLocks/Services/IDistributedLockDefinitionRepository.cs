using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.DistributedLocks
{
    /// <summary>
    /// A thin repository that validates and exposes all
    /// the IDistributedLockDefinition instaces registered 
    /// with Cofoundry. This should be registered as Singleton
    /// so the validation is only done once.
    /// </summary>
    public interface IDistributedLockDefinitionRepository
    {
        IDistributedLockDefinition Get<TDefinition>()
            where TDefinition : IDistributedLockDefinition;

        IEnumerable<IDistributedLockDefinition> GetAll();
    }
}
