using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.DistributedLocks.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.DistributedLocks.Registration
{
    public class DistributedLockDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IDistributedLockManager, DistributedLockManager>()
                .RegisterAll<IDistributedLockDefinition>()
                .RegisterSingleton<IDistributedLockDefinitionRepository, DistributedLockDefinitionRepository>()
                ;
        }
    }
}
