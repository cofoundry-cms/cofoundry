using Cofoundry.Domain.Tests.Shared;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

namespace Cofoundry.Domain.Tests.Integration;

/// <summary>
/// This application factory is used to create <see cref="IntegrationTestApplication"/>
/// instances that should be scoped to your tests. The factory itself is
/// scoped for the test session and will initialize, reset and seed the database
/// before starting the test session.
/// </summary>
public class IntegrationTestApplicationFactory : IAsyncLifetime
{
    const string ConnectionStringSetting = "Cofoundry:Database:ConnectionString";

    private SeededEntities? _seededEntities;
    private IServiceProvider? _serviceProvider;
    private MsSqlContainer? _msSqlContainer;

    /// <summary>
    /// Creates a new test application instance with a new DI
    /// service scope. The application should be disposed of
    /// when you are done with it.
    /// </summary>
    /// <param name="serviceConfiguration">
    /// Optional service configuration that runs just before the
    /// service collection is built, enabling you to override 
    /// services.
    /// </param>
    /// <returns>The newly created application instance.</returns>
    public virtual IntegrationTestApplication Create(Action<IServiceCollection>? serviceConfiguration = null)
    {
        if (_serviceProvider == null || _seededEntities == null)
        {
            throw new InvalidOperationException(nameof(IntegrationTestApplicationFactory) + " has not been initialized.");
        }

        var serviceProvider = _serviceProvider;
        if (serviceConfiguration != null)
        {
            // Only build a new service provider if we need to
            serviceProvider = IntegrationTestApplicationServiceProviderFactory.CreateTestHostProvider(
                additionalConfiguration: ConfigureDb,
                additionalServices: serviceConfiguration
                );
        }

        var app = new IntegrationTestApplication(serviceProvider, _seededEntities);

        return app;
    }

    /// <summary>
    /// Called by xUnit after the class has been created, this bootstraps
    /// the database at the start of the test session.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        await StartDbAsync();

        _serviceProvider = IntegrationTestApplicationServiceProviderFactory.CreateTestHostProvider(ConfigureDb);
        var dbInitializer = new TestDatabaseInitializer(_serviceProvider);
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

    public virtual async Task DisposeAsync()
    {
        if (_msSqlContainer != null)
        {
            await _msSqlContainer.DisposeAsync();
        }

        (_serviceProvider as IDisposable)?.Dispose();

        if (_serviceProvider is IAsyncDisposable asyncDisposableServiceProvider)
        {
            await asyncDisposableServiceProvider.DisposeAsync();
        }
    }
}
