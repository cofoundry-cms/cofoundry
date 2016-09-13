using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain
{
    public class UserAreaDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<UserAuthenticationHelper, UserAuthenticationHelper>()
                .RegisterType<UserCommandPermissionsHelper, UserCommandPermissionsHelper>();
        }
    }
}
