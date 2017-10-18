using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain
{
    public class UsersDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IUserAreaRepository, UserAreaRepository>()
                .RegisterType<IUserRepository, UserRepository>()
                .RegisterType<UserContextMapper>()
                .RegisterType<IResetUserPasswordCommandHelper, ResetUserPasswordCommandHelper>()
                .RegisterType<IUserContextService, UserContextService>()
                .RegisterType<ILoginService, LoginService>()
                .RegisterType<IUserMicroSummaryMapper, UserMicroSummaryMapper>()
                .RegisterType<IUserAccountDetailsMapper, UserAccountDetailsMapper>()
                .RegisterType<IUserDetailsMapper, UserDetailsMapper>()
                .RegisterAll<IUserAreaDefinition>()
                ;
        }
    }
}
