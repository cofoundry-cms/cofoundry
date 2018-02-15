using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.Json
{
    public class JsonDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.RegisterSingleton<IJsonSerializerSettingsFactory, JsonSerializerSettingsFactory>();
        }
    }
}
