using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
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
}
