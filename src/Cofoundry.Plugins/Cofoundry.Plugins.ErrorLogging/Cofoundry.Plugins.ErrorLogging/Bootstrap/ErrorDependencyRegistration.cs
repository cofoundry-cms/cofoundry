using Cofoundry.Core.DependencyInjection;
using Cofoundry.Plugins.ErrorLogging.Data;
using Cofoundry.Plugins.ErrorLogging.Domain;

namespace Cofoundry.Plugins.ErrorLogging.Bootstrap;

public class ErrorDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IErrorLoggingService, ErrorLoggingService>()
            .Register<ErrorLoggingDbContext>()
            ;
    }
}
