using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity.Registration
{
    public class IdentityDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<AccountManagementControllerHelper, AccountManagementControllerHelper>()
                .Register<AuthenticationControllerHelper, AuthenticationControllerHelper>()
                .Register<UserManagementControllerHelper, UserManagementControllerHelper>()
                ;

        }
    }
}
