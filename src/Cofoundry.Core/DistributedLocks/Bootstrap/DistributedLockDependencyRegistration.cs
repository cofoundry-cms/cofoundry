using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.DistributedLocks.Internal;

namespace Cofoundry.Core.DistributedLocks.Registration;

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
