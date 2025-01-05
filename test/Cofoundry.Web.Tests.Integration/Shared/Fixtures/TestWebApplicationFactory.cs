using Cofoundry.Core.Caching;
using Cofoundry.Domain.Tests.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace Cofoundry.Web.Tests.Integration;

/// <summary>
/// Implementation of <see cref="WebApplicationFactory{TEntryPoint}"/> designed to run in
/// the Cofoundry.Web.Tests.Integration project.
/// </summary>
public class TestWebApplicationFactory : TestWebApplicationFactory<Startup>
{
}

/// <summary>
/// Factory for bootstrapping an application in memory for functional end to end
/// tests.
/// </summary>
/// <typeparam name="TEntryPoint">
/// A type in the entry point assembly of the application. Typically the Startup
/// or Program classes can be used.
/// </typeparam>
public class TestWebApplicationFactory<TEntryPoint>
    : WebApplicationFactory<TEntryPoint>
    , IAsyncLifetime
    where TEntryPoint : class
{
    const string ConnectionStringSetting = "Cofoundry:Database:ConnectionString";

    private SeededEntities? _seededEntities;
    private MsSqlContainer? _msSqlContainer;

    protected override IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder();

        return builder;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // SeededEntities is added to the DI chain as it's used
        // by WebApplicationFactoryExtensions.CreateApp() 
        if (_seededEntities == null)
        {
            throw new InvalidOperationException("The factory must be initialized before configuring a web host.");
        }

        // https://github.com/dotnet/aspnetcore/issues/17707#issuecomment-609061917
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        builder.ConfigureAppConfiguration(builder =>
        {
            TestEnvironmentConfigurationBuilder.AddConfiguration(builder, ConfigureDb);
        });
        builder.UseStartup<TEntryPoint>();
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(_seededEntities);
        });

        base.ConfigureWebHost(builder);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        // Ensure that any shared cache has been cleared before we begin testing
        // as new data may have been generated by services in a separate service
        // collection
        var cache = Services.GetRequiredService<IObjectCacheFactory>();
        cache.Clear();
    }

    /// <summary>
    /// Called by xUnit after the class has been created, this bootstraps
    /// the database at the start of the test session.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        await StartDbAsync();

        using var serviceProvider = IntegrationTestApplicationServiceProviderFactory.CreateTestHostProvider(ConfigureDb);
        var dbInitializer = new TestDatabaseInitializer(serviceProvider);
        await dbInitializer.InitializeCofoundry();
        await dbInitializer.DeleteTestData();
        _seededEntities = await dbInitializer.SeedGlobalEntities();
    }

    private async Task StartDbAsync()
    {
        var configuration = TestEnvironmentConfigurationBuilder.BuildConfiguration();
        var connectionString = configuration.GetValue<string>(ConnectionStringSetting);

        if (string.IsNullOrEmpty(connectionString))
        {
            _msSqlContainer = new MsSqlBuilder().Build();
            await _msSqlContainer.StartAsync();
        }
    }

    private void ConfigureDb(IConfigurationBuilder configurationBuilder)
    {
        if (_msSqlContainer != null)
        {
            var connectionString = _msSqlContainer?.GetConnectionString();
            configurationBuilder.AddInMemoryCollection(
                [new(ConnectionStringSetting, connectionString)]
                );
        }
    }

    public new async Task DisposeAsync()
    {
        base.Dispose();

        if (_msSqlContainer != null)
        {
            await _msSqlContainer.DisposeAsync();
        }
    }
}
