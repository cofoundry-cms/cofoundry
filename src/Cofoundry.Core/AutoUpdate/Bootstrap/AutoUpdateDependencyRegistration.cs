using Cofoundry.Core.AutoUpdate.Internal;
using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Register<IAutoUpdateService, AutoUpdateService>()
                .Register<IUpdatePackageOrderer, UpdatePackageOrderer>()
                .Register<IAutoUpdateDistributedLockManager, AutoUpdateDistributedLockManager>()
                .RegisterAll<IUpdatePackageFactory>()
                ;
        }
    }
}
