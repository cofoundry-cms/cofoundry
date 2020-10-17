using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.Configuration.Registration
{
    public class ConfigurationDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var singleInstanceRegistrationOptions = RegistrationOptions.SingletonScope();

            container
                .RegisterGeneric(typeof(IConfigurationSettingsFactory<>), typeof(ConfigurationSettingsFactory<>), singleInstanceRegistrationOptions)
                .RegisterAllWithFactory(typeof(IConfigurationSettings), typeof(IConfigurationSettingsFactory<>), singleInstanceRegistrationOptions);
        }
    }
}
