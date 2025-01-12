namespace Cofoundry.Domain.Tests;

/// <summary>
/// A mock application that can be used for integration tests
/// outside of a web framework.
/// </summary>
public sealed class IntegrationTestApplication : IDisposable
{
    private readonly IServiceProvider _rootServiceProvider;

    public IntegrationTestApplication(
        IServiceProvider serviceProvider,
        SeededEntities seededEntities
        )
    {
        _rootServiceProvider = serviceProvider;
        Services = new TestApplicationServiceScope(_rootServiceProvider);
        SeededEntities = seededEntities;
        TestData = new TestDataHelper(_rootServiceProvider, SeededEntities);
        Mocks = new MockServicesHelper(Services);
    }

    /// <summary>
    /// A service provider scoped to the lifetime of this instance.
    /// </summary>
    public TestApplicationServiceScope Services { get; private set; }

    /// <summary>
    /// Used to make it easier to create or reference domain 
    /// entities in test fixtures.
    /// </summary>
    public TestDataHelper TestData { get; private set; }

    /// <summary>
    /// References to pre-seeded static/global test data. These can be used for convenience i
    /// n multiple tests as references but should not be altered.
    /// </summary>
    public SeededEntities SeededEntities { get; private set; }

    /// <summary>
    /// Convenience methods to make it easier to work
    /// with mocks and service wrappers set up in the 
    /// service collection for testing.
    /// </summary>
    public MockServicesHelper Mocks { get; set; }

    public void Dispose()
    {
        Services?.Dispose();
    }
}
