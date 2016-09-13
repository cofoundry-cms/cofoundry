using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    public class AutoUpdateDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IDatabase, Database>()
                .RegisterAllGenericImplementations(typeof(ISyncUpdateCommandHandler<>))
                .RegisterAllGenericImplementations(typeof(IAsyncUpdateCommandHandler<>))
                .RegisterType<IUpdateCommandHandlerFactory, UpdateCommandHandlerFactory>()
                .RegisterType<IAutoUpdateService, AutoUpdateService>()
                .RegisterAll<IUpdatePackageFactory>()
                ;
        }
    }
}
