using Cofoundry.Core.AutoUpdate.Internal;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.AutoUpdate.Registration
{
    public class AutoUpdateDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAllGenericImplementations(typeof(ISyncVersionedUpdateCommandHandler<>))
                .RegisterAllGenericImplementations(typeof(IAsyncVersionedUpdateCommandHandler<>))
                .RegisterAllGenericImplementations(typeof(ISyncAlwaysRunUpdateCommandHandler<>))
                .RegisterAllGenericImplementations(typeof(IAsyncAlwaysRunUpdateCommandHandler<>))
                .Register<IUpdateCommandHandlerFactory, UpdateCommandHandlerFactory>()
                .Register<IAutoUpdateStore, AutoUpdateStore>()
                .Register<IAutoUpdateService, AutoUpdateService>()
                .Register<IUpdatePackageOrderer, UpdatePackageOrderer>()
                .Register<IAutoUpdateDistributedLockManager, AutoUpdateDistributedLockManager>()
                .RegisterAll<IUpdatePackageFactory>()
                .RegisterAll<IStartupValidator>()
                ;
        }
    }
}