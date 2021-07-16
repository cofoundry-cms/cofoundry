using Cofoundry.Core.AutoUpdate;
using Cofoundry.Core.Time;
using Cofoundry.Core.Time.Mocks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Shared.Mocks;
using Cofoundry.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Fixture that uses a Cofoundry db that is shared between test instances and
    /// also includes a fully configured service provider. The database is initialized 
    /// and cleaned up when the fixture is initialized. 
    /// </summary>
    public class DbDependentFixture : IAsyncLifetime
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public async Task InitializeAsync()
        {
            ServiceProvider = CreateServiceProvider();
            await InitializeCofoundry();
            await Reset();
        }

        public MockServiceProviderScope CreateServiceScope()
        {
            return new MockServiceProviderScope(ServiceProvider);
        }

        private async Task InitializeCofoundry()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var autoUpdateService = scope.ServiceProvider.GetRequiredService<IAutoUpdateService>();
                await autoUpdateService.UpdateAsync();
            }
        }

        private ServiceProvider CreateServiceProvider()
        {
            // Here we create the service provider by utilising the
            // Cofoundry web bootstrapper, but it would be better
            // if we could improve this and do it without the web host
            // or make it more extensible for other test projects
            // e.g. plugins.

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

            services.AddScoped<IDateTimeService, MockDateTimeService>();
            services.AddScoped<IUserSessionService, InMemoryUserSessionService>();
            services.AddSingleton<IClientConnectionService>(new MockClientConnectionService(c => c.IPAddress = "127.0.0.1"));
            
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }

        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private async Task Reset()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CofoundryDbContext>();

                // reset data between tests to baseline
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    delete from Cofoundry.CustomEntity
                    delete from Cofoundry.DocumentAsset
                    delete from Cofoundry.DocumentAssetGroup
                    delete from Cofoundry.FailedAuthenticationAttempt
                    delete from Cofoundry.ImageAsset
                    delete from Cofoundry.ImageAssetGroup
                    delete from Cofoundry.[Page]
                    delete from Cofoundry.PageGroup
                    delete from Cofoundry.PageDirectory
                    delete from Cofoundry.PageTemplate
                    delete from Cofoundry.RewriteRule
                    delete from Cofoundry.Tag
                    delete from Cofoundry.UserLoginLog
                    delete from Cofoundry.UserPasswordResetRequest
                    delete Cofoundry.[User] where IsSystemAccount = 0
                    delete from Cofoundry.[Role] where RoleCode is null
                    delete from Cofoundry.UnstructuredDataDependency
                ");
            }
        }

        public async Task DisposeAsync()
        {
            if (ServiceProvider != null)
            {
                ServiceProvider.Dispose();
                await ServiceProvider.DisposeAsync();
            }
        }
    }
}
