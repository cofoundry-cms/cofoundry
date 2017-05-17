using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.Configuration
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterGeneric(typeof(IConfigurationSettingsFactory<>), typeof(ConfigurationSettingsFactory<>))
                .RegisterAllWithFactory(typeof(IConfigurationSettings), typeof(IConfigurationSettingsFactory<>));
        }
    }
}
