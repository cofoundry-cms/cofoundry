using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Internal;

namespace Cofoundry.Core.Registration
{
    public class CoreDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IEmailAddressNormalizer, EmailAddressNormalizer>()
                ;
        }
    }
}
