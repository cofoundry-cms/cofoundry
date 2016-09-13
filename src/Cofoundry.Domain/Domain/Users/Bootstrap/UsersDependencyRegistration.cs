using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain
{
    public class UsersDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            // TODO: I'm not sure why this is PerLifetime. I rember having issues with the scoping
            // but this should probably be tested out to make sure the scope is as expected.
            var perLifetimeScopeRegistrationOptions = new RegistrationOptions() { InstanceScope = InstanceScope.PerLifetimeScope };

            container
                .RegisterType<IUserAreaRepository, UserAreaRepository>()
                .RegisterType<UserContextMapper>()
                .RegisterType<IResetUserPasswordCommandHelper, ResetUserPasswordCommandHelper>()
                .RegisterType<IUserContextService, UserContextService>(perLifetimeScopeRegistrationOptions)
                .RegisterAll<IUserArea>()
                .RegisterFactory<AuthenticationSettings, ConfigurationSettingsFactory<AuthenticationSettings>>();
        }
    }
}
