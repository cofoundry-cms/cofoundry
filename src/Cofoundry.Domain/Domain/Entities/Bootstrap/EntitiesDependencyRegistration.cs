using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class EntitiesDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .RegisterAll<IEntityDefinition>(RegistrationOptions.SingletonScope())
            .RegisterSingleton<IEntityDefinitionRepository, EntityDefinitionRepository>()
            .Register<IDependableEntityDeleteCommandValidator, DependableEntityDeleteCommandValidator>();
    }
}
