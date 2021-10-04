using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// A convenient wrapper around an <see cref="IServiceScope"/>
    /// to make it easier to reference services in tests.
    /// </summary>
    public class TestApplicationServiceScope : IServiceProvider, IDisposable
    {
        private readonly IServiceScope _baseServiceProviderScope;

        public TestApplicationServiceScope(
            IServiceProvider baseServiceProvider
            )
        {
            _baseServiceProviderScope = baseServiceProvider.CreateScope();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType. -or- null if there is no service object
        /// of type serviceType.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return _baseServiceProviderScope.ServiceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Gets an instance of <see cref="IAdvancedContentRepository"/>
        /// from the service provider. Typically you'd only use this
        /// when testing permissions, otherwise you'd be better off
        /// calling <see cref="GetContentRepositoryWithElevatedPermissions"/>
        /// </summary>
        public IAdvancedContentRepository GetContentRepository()
        {
            return this.GetRequiredService<IAdvancedContentRepository>();
        }

        /// <summary>
        /// Gets an instance of <see cref="IAdvancedContentRepository"/>
        /// from the service provider and elevates it to run under the
        /// system user account. Use this whenever you're not testing 
        /// permissions.
        /// </summary>
        public IAdvancedContentRepository GetContentRepositoryWithElevatedPermissions()
        {
            return this
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();
        }

        public void Dispose()
        {
            _baseServiceProviderScope?.Dispose();
        }
    }
}
