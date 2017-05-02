using Cofoundry.Core.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using SandboxDependency.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SandboxDependency
{
    public static class CofoundryServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the dependency resolver for Cofoundry to use AutoFac and
        /// registers all the services, repositories and modules setup for auto-registration.
        /// </summary>
        public static IMvcBuilder AddCofoundry(this IServiceCollection services)
        {
            // Assume add MVC? or add it ourselves

            //services.Configure()
            var mvcBuilder = services.AddMvc();

            DiscoverAdditionalApplicationParts(mvcBuilder);

            var typesProvider = new DiscoveredTypesProvider(mvcBuilder.PartManager);
            var builder = new DefaultContainerBuilder(services, typesProvider);
            builder.Build();

            //var registrations = container.Resolve<IEnumerable<IAssemblyResourceRegistration>>();

            return mvcBuilder;
        }

        private static void DiscoverAdditionalApplicationParts(IMvcBuilder mvcBuilder)
        {
            var assemblyPartDiscoveryProvider = new AssemblyDiscoveryProvider();
            var rules = new IAssemblyDiscoveryRule[] { new CofoundryAssemblyDiscoveryRule() };

            var additionalAssemblies = assemblyPartDiscoveryProvider.DiscoverAssemblies(mvcBuilder, rules);

            foreach (var additionalAssembly in additionalAssemblies)
            {
                mvcBuilder.AddApplicationPart(additionalAssembly);
            }
        }

        //private static T GetServiceFromCollection<T>(IServiceCollection services)
        //{
        //    return (T)services
        //        .LastOrDefault(d => d.ServiceType == typeof(T))
        //        ?.ImplementationInstance;
        //}
    }
}
