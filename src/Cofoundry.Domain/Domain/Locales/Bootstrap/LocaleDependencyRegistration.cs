using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class LocaleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<ILocaleCache, LocaleCache>()
            .Register<IActiveLocaleMapper, ActiveLocaleMapper>()
            ;
    }
}
