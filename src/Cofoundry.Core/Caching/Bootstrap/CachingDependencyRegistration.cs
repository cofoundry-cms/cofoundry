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
            container
                .RegisterSingleton<IObjectCacheFactory, InMemoryObjectCacheFactory>()
                ;
        }
    }
}
