using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.Framework.Mvc.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cofoundry.Web
{
    public static class AddCofoundryStartupExtension
    {
        /// <summary>
        /// Configures the dependency resolver for Cofoundry and
        /// registers all the services, repositories and modules setup for auto-registration.
        /// </summary>
        public static IMvcBuilder AddCofoundry(
            this IMvcBuilder mvcBuilder,
            IConfiguration configuration,
            Action<AddCofoundryStartupConfiguration> configBuilder = null
            )
        {
            var cofoundryConfig = new AddCofoundryStartupConfiguration();
            configBuilder?.Invoke(cofoundryConfig);

            AddAdditionalTypes(mvcBuilder);
            DiscoverAdditionalApplicationParts(mvcBuilder, cofoundryConfig);

            var typesProvider = new DiscoveredTypesProvider(mvcBuilder.PartManager);

            var builder = new DefaultContainerBuilder(mvcBuilder.Services, typesProvider, configuration);

            builder.Build();

            RunAdditionalConfiguration(mvcBuilder);
            if (cofoundryConfig.EnableLocalization)
            {
                ConfigureLocalization(mvcBuilder, cofoundryConfig);
            }

            return mvcBuilder;
        }

        private static void AddAdditionalTypes(IMvcBuilder mvcBuilder)
        {
            // Ensure IHttpContextAccessor is added, because it isn't by default
            // see https://github.com/aspnet/Hosting/issues/793
            mvcBuilder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        private static void DiscoverAdditionalApplicationParts(IMvcBuilder mvcBuilder, AddCofoundryStartupConfiguration cofoundryConfig)
        {
            // We could configure AssemblyDiscoveryProvider through settings?

            var assemblyPartDiscoveryProvider = new AssemblyDiscoveryProvider();

            var additionalAssemblies = assemblyPartDiscoveryProvider.DiscoverAssemblies(mvcBuilder, cofoundryConfig.AssemblyDiscoveryRules);

            foreach (var additionalAssembly in additionalAssemblies)
            {
                mvcBuilder.AddApplicationPart(additionalAssembly);
            }
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
                var mvcBuilderConfigurations = serviceScope.ServiceProvider.GetRequiredService<IEnumerable<IStartupServiceConfigurationTask>>();
                foreach (var mvcBuilderConfiguration in mvcBuilderConfigurations)
                {
                    mvcBuilderConfiguration.ConfigureServices(mvcBuilder);
                }
            }
        }

        private static void ConfigureLocalization(IMvcBuilder mvcBuilder, AddCofoundryStartupConfiguration addCofoundryStartupConfiguration)
        {
            var serviceProvider = mvcBuilder.Services.BuildServiceProvider();
            List<ActiveLocale> locales;
            var queryExecutor = serviceProvider.GetService<IQueryExecutor>();
            try
            {
                locales = queryExecutor?.ExecuteAsync(new GetAllActiveLocalesQuery(), new ExecutionContext
                {
                    UserContext = new UserContext(),
                    ExecutionDate = DateTime.UtcNow
                }).GetAwaiter()
                    .GetResult()
                    .ToList() ?? new List<ActiveLocale>();
            }
            catch (Exception)
            {
                locales = GetDefaultLocales(addCofoundryStartupConfiguration);
            }

            if (!locales.Any())
            {
                locales = GetDefaultLocales(addCofoundryStartupConfiguration);
            }

            mvcBuilder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = locales.Select(x => new CultureInfo(x.IETFLanguageTag)).ToList();
                options.DefaultRequestCulture = addCofoundryStartupConfiguration.DefaultRequestCulture;
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new DefaultRequesCultureProvider
                {
                    Options = options
                });
            });
        }

        private static List<ActiveLocale> GetDefaultLocales(AddCofoundryStartupConfiguration addCofoundryStartupConfiguration)
        {
            return new List<ActiveLocale>
            {
                new ActiveLocale
                {
                    IETFLanguageTag = addCofoundryStartupConfiguration.DefaultRequestCulture.Culture.Name
                }
            };
        }
    }
}