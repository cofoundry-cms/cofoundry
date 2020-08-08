using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Json.Internal;

namespace Cofoundry.Core.Json.DependencyRegistration
{
    public class JsonDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.RegisterSingleton<IJsonSerializerSettingsFactory, JsonSerializerSettingsFactory>();
        }
    }
}
