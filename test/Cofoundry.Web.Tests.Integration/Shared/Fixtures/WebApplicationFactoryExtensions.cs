using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Cofoundry.Web.Tests.Integration
{
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
        /// <typeparam name="TEntryPoint"></typeparam>
        /// <param name="serviceConfiguration">Service configuration delegate to run after all other services have been resgistered.</param>
        public static HttpClient CreateClientWithServices<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, Action<IServiceCollection> serviceConfiguration)
            where TEntryPoint : class
        {
            return factory.WithServices(serviceConfiguration).CreateClient();
        }
    }
}
