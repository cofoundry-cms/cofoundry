using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Cofoundry.Web.Tests.Integration;

/// <summary>
/// Extensions to cut down on boilerplate when working with <see cref="WebApplicationFactory"/>
/// </summary>
public static class WebApplicationFactoryExtensions
{
    /// <summary>
    /// Creates a new <see cref="WebApplicationFactory"/> configured to override 
    /// services using the specific <paramref name="serviceConfiguration"/> delegate.
    /// This is a shortcut to running 
    /// <code>factory.WithWebHostBuilder(b => b.ConfigureTestServices(serviceConfiguration))</code>.
    /// </summary>
    /// <param name="serviceConfiguration">Service configuration delegate to run after all other services have been resgistered.</param>
    /// <returns>A new <see cref="WebApplicationFactory"/> with the altered configuration.</returns>
    public static WebApplicationFactory<TEntryPoint> WithServices<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, Action<IServiceCollection> serviceConfiguration)
        where TEntryPoint : class
    {
        return factory.WithWebHostBuilder(b => b.ConfigureTestServices(serviceConfiguration));
    }

    /// <summary>
    /// Creates a new HttpClient instance for the test application, having
    /// configured it with the specified service overrides.
    /// </summary>
    /// <param name="serviceConfiguration">Service configuration delegate to run after all other services have been resgistered.</param>
    public static HttpClient CreateClientWithServices<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, Action<IServiceCollection> serviceConfiguration)
        where TEntryPoint : class
    {
        return factory.WithServices(serviceConfiguration).CreateClient();
    }

    /// <summary>
    /// Shortcut to creating a new client with configurable options. The default options
    /// allow for redirects and handle cookies.
    /// </summary>
    /// <param name="configureClientOptions">
    /// Option configuration for a new instance of <see cref="WebApplicationFactoryClientOptions"/>. 
    /// The default options allow for redirects and handle cookies.
    /// </param>
    public static HttpClient CreateClient<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, Action<WebApplicationFactoryClientOptions> configureClientOptions)
       where TEntryPoint : class
    {
        var options = new WebApplicationFactoryClientOptions();

        if (configureClientOptions != null)
        {
            configureClientOptions(options);
        }

        return factory.CreateClient(options);
    }

    /// <summary>
    /// <para>
    /// Creates a new <see cref="DbDependentTestApplication"/> instance
    /// which can be used to create and work with test entities directly
    /// through the domain layer with the same API used in the domain
    /// integration tests project.
    /// </para>
    /// <para>
    /// Note that although the test app and client instances share the same base
    /// service configuration, they don't seem to share the service collection and
    /// therefore the same singleton instances, so be aware that features like in-memory
    /// caching are not shared. To get around this, the caching services in the client
    /// app will be reset when a new client is created.
    /// </para>
    /// <para>
    /// The application should be disposed of
    /// when you are done with it.
    /// </para>
    /// </summary>
    public static DbDependentTestApplication CreateApp<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory)
        where TEntryPoint : class
    {
        var seededEntities = factory.Services.GetRequiredService<SeededEntities>();

        var factoryWithAppDependencies = factory.WithServices(services =>
        {
            DbDependentTestApplicationServiceProviderFactory.ConfigureTestServices(services);
        });

        var app = new DbDependentTestApplication(factoryWithAppDependencies.Services, seededEntities);

        return app;
    }
}
