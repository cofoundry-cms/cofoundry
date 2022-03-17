using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.Configuration.Registration;

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
