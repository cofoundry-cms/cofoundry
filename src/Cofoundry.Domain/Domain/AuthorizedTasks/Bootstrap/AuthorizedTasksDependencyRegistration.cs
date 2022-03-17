using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class AuthorizedTasksDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .RegisterAll<IAuthorizedTaskTypeDefinition>()
            .RegisterSingleton<IAuthorizedTaskTypeDefinitionRepository, AuthorizedTaskTypeDefinitionRepository>()
            .Register<IAuthorizedTaskTokenFormatter, AuthorizedTaskTokenFormatter>()
            .Register<IAuthorizedTaskAuthorizationCodeGenerator, AuthorizedTaskAuthorizationCodeGenerator>()
            ;
    }
}
