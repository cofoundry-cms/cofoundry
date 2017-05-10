using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    public class DefaultContainerResolutionContext : IResolutionContext
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultContainerResolutionContext(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public IChildResolutionContext CreateChildContext()
        {
            return new DefaultContainerChildResolutionContext(_serviceProvider.CreateScope());
        }

        public bool IsRegistered<T>()
        {
            var service = _serviceProvider.GetService<T>();
            return service != null;
        }

        public bool IsRegistered(Type typeToCheck)
        {
            var service = _serviceProvider.GetService(typeToCheck);
            return service != null;
        }

        public TService Resolve<TService>()
        {
            var service = _serviceProvider.GetService<TService>();
            return service;
        }

        public object Resolve(Type t)
        {
            var service = _serviceProvider.GetService(t);
            return service;
        }

        public IEnumerable<TService> ResolveAll<TService>()
        {
            var services = _serviceProvider.GetServices<TService>();
            return services;
        }
    }
}
