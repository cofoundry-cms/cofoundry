using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class UserAreaDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<UserAuthenticationHelper, UserAuthenticationHelper>()
                .Register<UserCommandPermissionsHelper, UserCommandPermissionsHelper>()
                ;
        }
    }
}
