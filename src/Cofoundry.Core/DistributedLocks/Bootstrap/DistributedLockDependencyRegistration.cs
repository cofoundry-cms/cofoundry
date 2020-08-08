using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.DistributedLocks.DependencyRegistration
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
