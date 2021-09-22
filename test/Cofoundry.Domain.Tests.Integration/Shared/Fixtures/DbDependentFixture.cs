using Cofoundry.Core.AutoUpdate;
using Cofoundry.Core.MessageAggregator;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Fixture that uses a Cofoundry db that is shared between test instances and
    /// also includes a fully configured service provider. The database is initialized 
    /// and cleaned up when the fixture is initialized. 
    /// </summary>
    /// <remarks>
    /// I expect we'll want to tidy this up and make certain parts reusable, but let's
    /// wait until we have more tests in place here and across other modules to get a feel 
    /// for how it might be refactored.
    /// </remarks>
    public class DbDependentFixture : IAsyncLifetime
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public SeededEntities SeededEntities { get; set; } = new SeededEntities();

        public async Task InitializeAsync()
        {
            ServiceProvider = CreateServiceProvider();
            await InitializeCofoundry();
            await Reset();
            await LoadGlobalEntityReferences();
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

        /// <summary>
        /// Here we create the service provider by utilising the
        /// Cofoundry web bootstrapper, but it would be better
        /// if we could improve this and do it without the web host
        /// or make it more extensible for other test projects
        /// e.g. plugins.
        /// </summary>
        public ServiceProvider CreateServiceProvider(Action<ServiceCollection> customServiceConfiguration = null)
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

            services.AddScoped<IDateTimeService, MockDateTimeService>();
            services.AddScoped<IUserSessionService, InMemoryUserSessionService>();
            services.AddScoped<IImageAssetFileService, MockImageAssetFileService>();
            services.AddScoped<IMessageAggregator, AuditableMessageAggregator>();
            services.AddSingleton<IClientConnectionService>(new MockClientConnectionService(c => c.IPAddress = "127.0.0.1"));

            if (customServiceConfiguration != null)
            {
                customServiceConfiguration(services);
            }

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

        /// <summary>
        /// Resets the database test data between test runs, leaving any
        /// definitions or global test entities in place.
        /// </summary>
        private async Task Reset()
        {
            using var scope = ServiceProvider.CreateScope();
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
                delete from Cofoundry.PageTemplate where [FileName] not in ('TestTemplate', 'TestCustomEntityTemplate')
                delete from Cofoundry.RewriteRule
                delete from Cofoundry.Tag
                delete from Cofoundry.UserLoginLog
                delete from Cofoundry.UserPasswordResetRequest
                delete Cofoundry.[User] where IsSystemAccount = 0
                delete from Cofoundry.[Role] where RoleCode is null
                delete from Cofoundry.UnstructuredDataDependency
            ");
        }

        /// <summary>
        /// Initializes any static/global test data references. These references can
        /// be used for convenience in multiple tests as references but should not be altered.
        /// </summary>
        private async Task LoadGlobalEntityReferences()
        {
            using var scope = ServiceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var dbContext = scope.ServiceProvider.GetRequiredService<CofoundryDbContext>();

            // Page Templates

            SeededEntities.TestPageTemplateId = await dbContext
                .PageTemplates
                .Where(t => t.FileName == "TestTemplate")
                .Select(t => t.PageTemplateId)
                .SingleAsync();

            SeededEntities.TestCustomEntityPageTemplateId = await dbContext
                .PageTemplates
                .Where(t => t.FileName == "TestCustomEntityTemplate")
                .Select(t => t.PageTemplateId)
                .SingleAsync();

            // Images

            var mockImageAssetFileService = scope.ServiceProvider.GetService<IImageAssetFileService>() as MockImageAssetFileService;
            mockImageAssetFileService.SaveFile = false;
            mockImageAssetFileService.WidthInPixels = 80;
            mockImageAssetFileService.HeightInPixels = 80;

            SeededEntities.TestImageId = await contentRepository
                .ImageAssets()
                .AddAsync(new AddImageAssetCommand()
                {
                    Title = "Test Image",
                    File = new EmbeddedResourceFileSource(this.GetType().Assembly, "Cofoundry.Domain.Tests.Integration.Shared.SeedData.Images", "Test jpg 80x80.jpg")
                });

            // Tags

            var testTag = new Tag()
            {
                TagText = SeededEntities.TestTag,
                CreateDate = DateTime.UtcNow
            };

            dbContext.Tags.Add(testTag);
            await dbContext.SaveChangesAsync();

            SeededEntities.TestTagId = testTag.TagId;

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
