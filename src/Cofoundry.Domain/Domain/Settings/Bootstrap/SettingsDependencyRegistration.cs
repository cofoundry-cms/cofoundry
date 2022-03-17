using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class SettingsDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<ISettingCache, SettingCache>()
            .Register<SettingQueryHelper>()
            .Register<SettingCommandHelper>()
            .Register<IInternalSettingsRepository, InternalSettingsRepository>()
            ;
    }
}
