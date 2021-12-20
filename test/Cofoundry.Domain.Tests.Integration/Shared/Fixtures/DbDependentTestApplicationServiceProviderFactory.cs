using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Time;
using Cofoundry.Core.Time.Mocks;
using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Integration.Mocks;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Mocks;
using Cofoundry.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Creates a service provider specifically for the 
    /// <see cref="DbDependentTestApplicationFactory"/> that includes the
    /// Cofoundry DI bootstrapper and a handful of useful mock services
    /// to make testing easier.
    /// </summary>
    /// <remarks>
    /// Here we utilize the Cofoundry web bootstrapper which makes it a little
    /// inelegant in setting up the provider with some of the web dependencies.
    /// It would be better  if we could improve this and do it without the web host
    /// or make it more extensible for other test projects e.g. plugins.
    /// </remarks>
    public static class DbDependentTestApplicationServiceProviderFactory
    {
        /// <summary>
        /// Creates a service provider specifically for the 
        /// <see cref="DbDependentTestApplicationFactory"/> that includes the
        /// Cofoundry DI bootstrapper and a handful of useful mock services
        /// to make testing easier.
        /// </summary>
        /// <param name="customServiceConfiguration">
        /// Optional service configuration to run after the tets services are added.
        /// </param>
        public static ServiceProvider CreateTestHostProvider(Action<IServiceCollection> customServiceConfiguration = null)
        {
            var configuration = GetConfiguration();
            var services = new ServiceCollection();
            var hostEnvironment = new TestHostEnvironemnt();
            services.AddSingleton<IHostEnvironment>(hostEnvironment);
            services.AddSingleton<IWebHostEnvironment>(hostEnvironment);
            var listener = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(listener);
            services.AddSingleton(listener);
            services.AddSingleton(configuration);
            services
                .AddLogging(config => config.AddDebug().AddConsole())
                .AddHttpClient()
                .AddControllersWithViews()
                .AddCofoundry(configuration);

            ConfigureTestServices(services, customServiceConfiguration);

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }

        /// <summary>
        /// Configures an existing service provider to include a handful of useful 
        /// mock services to make testing easier.
        /// </summary>
        /// <param name="services">Service collection to modify.</param>
        /// <param name="customServiceConfiguration">
        /// Optional service configuration to run after the tets services are added.
        /// </param>
        public static void ConfigureTestServices(IServiceCollection services, Action<IServiceCollection> customServiceConfiguration = null)
        {
            services.AddScoped<IDateTimeService, MockDateTimeService>();
            services.AddScoped<IUserSessionService, InMemoryUserSessionService>();
            services.AddScoped<IImageAssetFileService, MockImageAssetFileService>();
            services.AddScoped<IMessageAggregator, AuditableMessageAggregator>();
            services.AddScoped<IMailDispatchSession, AuditableMailDispatchSession>();
            services.AddScoped<IPageTemplateViewFileLocator, TestPageTemplateViewFileLocator>();
            services.AddSingleton<IClientConnectionService>(new MockClientConnectionService(c => c.IPAddress = TestIPAddresses.Localhost));
            services.AddTransient<IViewFileReader, TestViewFileReader>();

            if (customServiceConfiguration != null)
            {
                customServiceConfiguration(services);
            }
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
