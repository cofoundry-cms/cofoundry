using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
    public class EntitiesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IEntityDefinition>(RegistrationOptions.SingletonScope())
                .RegisterInstance<IEntityDefinitionRepository, EntityDefinitionRepository>();
        }
    }
}
