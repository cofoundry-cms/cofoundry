using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Extendable;

namespace Cofoundry.Core.Registration;

public class CoreDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IEmailAddressNormalizer, EmailAddressNormalizer>()
            .Register<IEmailAddressUniquifier, EmailAddressUniquifier>()
            .Register<IUsernameNormalizer, UsernameNormalizer>()
            .Register<IUsernameUniquifier, UsernameUniquifier>()
            ;
    }
}
