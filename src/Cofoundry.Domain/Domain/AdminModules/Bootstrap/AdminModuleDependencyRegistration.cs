using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Registration;

public class AdminModuleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container.RegisterAll<IAdminModuleRegistration>();
    }
}
