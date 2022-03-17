namespace Cofoundry.Domain.Tests.Integration;

/// <summary>
/// This application factory is used to create <see cref="DbDependentTestApplication"/>
/// instances that should be scoped to your tests. The factory itself is
/// scoped for the test session and will initialize, reset and seed the database
/// before starting the test session.
/// </summary>
public class DbDependentTestApplicationFactory : IAsyncLifetime
{
    private SeededEntities _seededEntities;
    private IServiceProvider _serviceProvider;

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
    public virtual DbDependentTestApplication Create(Action<IServiceCollection> serviceConfiguration = null)
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException(nameof(DbDependentTestApplicationFactory) + " has not been initialized.");
        }

        var serviceProvider = _serviceProvider;
        if (serviceConfiguration != null)
        {
            // Only build a new service provider if we need to
            serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider(serviceConfiguration);
        }

        var app = new DbDependentTestApplication(serviceProvider, _seededEntities);

        return app;
    }

    /// <summary>
    /// Called by xUnit after the class has been created, this bootstraps
    /// the database at the start of the test session.
    /// </summary>
    /// <returns></returns>
    public virtual async Task InitializeAsync()
    {
        _serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
        var dbInitializer = new TestDatabaseInitializer(_serviceProvider);
        await dbInitializer.InitializeCofoundry();
        await dbInitializer.DeleteTestData();
        _seededEntities = await dbInitializer.SeedGlobalEntities();
    }

    public virtual async Task DisposeAsync()
    {
        (_serviceProvider as IDisposable)?.Dispose();

        var asyncDisposableServiceProvider = _serviceProvider as IAsyncDisposable;
        if (asyncDisposableServiceProvider != null)
        {
            await asyncDisposableServiceProvider.DisposeAsync();
        }
    }
}
