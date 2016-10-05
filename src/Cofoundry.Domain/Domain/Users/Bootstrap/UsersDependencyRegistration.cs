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
            // Per lifetime because in MVC filters, controllers etc don't resolve from a single lifetime context
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
