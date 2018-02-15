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
                .Register<IUserAreaDefinitionRepository, UserAreaDefinitionRepository>()
                .Register<IUserRepository, UserRepository>()
                .Register<UserContextMapper>()
                .Register<IResetUserPasswordCommandHelper, ResetUserPasswordCommandHelper>()
                .Register<IUserContextService, UserContextService>(RegistrationOptions.Scoped())
                .Register<ILoginService, LoginService>()
                .Register<IUserMicroSummaryMapper, UserMicroSummaryMapper>()
                .Register<IUserSummaryMapper, UserSummaryMapper>()
                .Register<IUserAccountDetailsMapper, UserAccountDetailsMapper>()
                .Register<IUserDetailsMapper, UserDetailsMapper>()
                .RegisterAll<IUserAreaDefinition>()
                ;
        }
    }
}
