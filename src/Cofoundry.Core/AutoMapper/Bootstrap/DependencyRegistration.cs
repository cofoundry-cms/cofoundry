using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.AutoMapper
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<Profile>()
                .RegisterType<IAutoMapBootstrapper, AutoMapBootstrapper>()
                .RegisterFactory<IMapper, MappingEngineInjectionFactory>()
                ;
        }
    }
}
