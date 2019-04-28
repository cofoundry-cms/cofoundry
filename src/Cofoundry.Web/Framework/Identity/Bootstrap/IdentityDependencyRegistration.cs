using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity
{
    public class IdentityDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<AccountManagementControllerHelper, AccountManagementControllerHelper>()
                .RegisterGeneric(typeof(IAuthenticationControllerHelper<>), typeof(AuthenticationControllerHelper<>))
                .Register<UserManagementControllerHelper, UserManagementControllerHelper>()
                ;
        }
    }
}
