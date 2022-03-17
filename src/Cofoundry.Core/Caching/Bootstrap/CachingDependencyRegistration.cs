using Cofoundry.Core.Caching.Internal;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.Caching.Registration;

public class CachingDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        var cacheMode = container.Configuration.GetValue("Cofoundry:InMemoryObjectCache:CacheMode", InMemoryObjectCacheMode.Persistent);

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
