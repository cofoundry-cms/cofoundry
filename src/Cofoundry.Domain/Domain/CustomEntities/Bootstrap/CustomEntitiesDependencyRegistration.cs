using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain
{
    public class CustomEntitiesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<ICustomEntityDisplayModelMapper, CustomEntityDisplayModelMapper>()
                .RegisterType<CustomEntityDataModelMapper>()
                .RegisterType<ICustomEntityCache, CustomEntityCache>()
                .RegisterType<ICustomEntityRepository, CustomEntityRepository>()
                .RegisterAll<ICustomEntityRoutingRule>()
                .RegisterAll<ICustomEntityDefinition>()
                .RegisterAll<ICustomEntityDisplayModel>()
                .RegisterAllGenericImplementations(typeof(ICustomEntityDetailsDisplayModelMapper<,>))
                .RegisterInstance<ICustomEntityCodeDefinitionRepository, CustomEntityCodeDefinitionRepository>()
                ;
        }
    }
}
