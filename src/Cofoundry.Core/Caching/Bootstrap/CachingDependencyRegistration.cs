using Cofoundry.Core.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching
{
    public class CachingDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterInstance<IObjectCacheFactory, InMemoryObjectCacheFactory>()
                ;
        }
    }
}
