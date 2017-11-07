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
            var singletonOptions = RegistrationOptions.SingletonScope();

            container
                .RegisterType<ICustomEntityDisplayModelMapper, CustomEntityDisplayModelMapper>()
                .RegisterType<ICustomEntityDataModelMapper, CustomEntityDataModelMapper>()
                .RegisterType<ICustomEntityCache, CustomEntityCache>()
                .RegisterType<ICustomEntityRepository, CustomEntityRepository>()
                .RegisterAll<ICustomEntityRoutingRule>(singletonOptions)
                .RegisterAll<ICustomEntityDefinition>(singletonOptions)
                .RegisterAll<ICustomEntityDisplayModel>()
                .RegisterAllGenericImplementations(typeof(ICustomEntityDisplayModelMapper<,>))
                .RegisterInstance<ICustomEntityDefinitionRepository, CustomEntityDefinitionRepository>()
                .RegisterType<ICustomEntityRenderSummaryMapper, CustomEntityRenderSummaryMapper>()
                .RegisterType<ICustomEntitySummaryMapper, CustomEntitySummaryMapper>()
                .RegisterType<ICustomEntityVersionSummaryMapper, CustomEntityVersionSummaryMapper>()
                .RegisterType<ICustomEntityDefinitionMicroSummaryMapper, CustomEntityDefinitionMicroSummaryMapper>()
                .RegisterType<ICustomEntityDefinitionSummaryMapper, CustomEntityDefinitionSummaryMapper>()
                .RegisterType<ICustomEntityRouteMapper, CustomEntityRouteMapper>()
                ;
        }
    }
}
