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
                .RegisterType<AccountManagementControllerHelper, AccountManagementControllerHelper>()
                .RegisterType<AuthenticationControllerHelper, AuthenticationControllerHelper>()
                .RegisterType<UserManagementControllerHelper, UserManagementControllerHelper>()
                ;

        }
    }
}
