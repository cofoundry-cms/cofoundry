using Cofoundry.Core.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public static class CofoundryStartupExtensions
    {
        /// <summary>
        /// Registers Cofoundry into the Owin pipeline and runs all the registered
        /// Cofoundry StartupTasks. You must install and register a Cofoundry DI Integration 
        /// nuget package before calling this method (e.g. app.UseCofoundryAutoFacIntegration())
        /// </summary>
        /// <param name="application">Application configuration</param>
        /// <param name="configBuilder">Additional configuration options</param>
        public static void UseCofoundry(this IApplicationBuilder application, Action<CofoundryStartupConfiguration> configBuilder = null)
        {
            var configuration = new CofoundryStartupConfiguration();
            if (configBuilder != null) configBuilder(configuration);

            using (var childContext = application.ApplicationServices.CreateScope())
            {
                // Use the fullname secondry ordering here to get predicatable task ordering
                IEnumerable<IStartupTask> startupTasks = childContext
                    .ServiceProvider
                    .GetServices<IStartupTask>()
                    .OrderBy(i => i.Ordering)
                    .ThenBy(i => i.GetType().FullName);

                if (configuration.StartupTaskFilter != null)
                {
                    startupTasks = configuration.StartupTaskFilter(startupTasks);
                }

                foreach (var startupTask in startupTasks)
                {
                    startupTask.Run(application);
                }
            }
        }

        /// <summary>
        /// Configures the dependency resolver for Cofoundry to use AutoFac and
        /// registers all the services, repositories and modules setup for auto-registration.
        /// </summary>
        public static IMvcBuilder AddCofoundry(this IMvcBuilder mvcBuilder)
        {
            DiscoverAdditionalApplicationParts(mvcBuilder);

            var typesProvider = new DiscoveredTypesProvider(mvcBuilder.PartManager);
            var builder = new DefaultContainerBuilder(mvcBuilder.Services, typesProvider);
            builder.Build();

            RunAdditionalConfiguration(mvcBuilder);

            return mvcBuilder;
        }

        /// <summary>
        /// MVC does not do a very good job of modular configurations, so here
        /// we have to prematurely build the container and use a child scope to 
        /// run additional configurations based on what has already been setup in the
        /// DI container. This allows for additional configuration to be made in
        /// plugins.
        /// </summary>
        private static void RunAdditionalConfiguration(IMvcBuilder mvcBuilder)
        {
            var serviceProvider = mvcBuilder.Services.BuildServiceProvider();
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var mvcBuilderConfiguration = serviceScope.ServiceProvider.GetRequiredService<IMvcBuilderConfiguration>();
                mvcBuilderConfiguration.Configure(mvcBuilder);
            }
        }

        private static void DiscoverAdditionalApplicationParts(IMvcBuilder mvcBuilder)
        {
            // We could configure AssemblyDiscoveryProvider through settings?

            var assemblyPartDiscoveryProvider = new AssemblyDiscoveryProvider();
            var rules = new IAssemblyDiscoveryRule[] { new CofoundryAssemblyDiscoveryRule() };

            var additionalAssemblies = assemblyPartDiscoveryProvider.DiscoverAssemblies(mvcBuilder, rules);

            foreach (var additionalAssembly in additionalAssemblies)
            {
                mvcBuilder.AddApplicationPart(additionalAssembly);
            }
        }
    }
}