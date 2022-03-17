using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class CultureDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<ICultureFactory, CultureFactory>()
            .Register<ICultureContextService, CultureContextService>()
            ;
    }
}
