using Cofoundry.Core.Caching.Internal;
using Cofoundry.Core.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching.Registration
{
    public class CachingDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var cacheMode = container.Configuration.GetValue("Cofoundry:InMemoryObjectCache:CacheMode", InMemoryObjectCacheMode.Persitent);

            if (cacheMode == InMemoryObjectCacheMode.PerScope)
            {
                container.RegisterScoped<IObjectCacheFactory, InMemoryObjectCacheFactory>();
            }
            else
            {
                container.RegisterSingleton<IObjectCacheFactory, InMemoryObjectCacheFactory>();
            }
        }
    }
}
