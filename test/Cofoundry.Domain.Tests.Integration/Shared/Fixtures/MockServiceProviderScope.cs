using Cofoundry.Core.Time;
using Cofoundry.Core.Time.Mocks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Integration
{
    public class MockServiceProviderScope : IServiceProvider, IDisposable
    {
        private readonly IServiceScope _baseServiceProviderScope;

        public MockServiceProviderScope(
            IServiceProvider baseServiceProvider
            )
        {
            ServiceProvider = baseServiceProvider;
            _baseServiceProviderScope = ServiceProvider.CreateScope();
        }

        public object GetService(Type serviceType)
        {
            return _baseServiceProviderScope.ServiceProvider.GetService(serviceType);
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public void MockDateTime(DateTime utcNow)
        {
            // If the service is already set up, just adjust the time.
            var dateTimeService = _baseServiceProviderScope.ServiceProvider.GetService<IDateTimeService>() as MockDateTimeService;
            if (dateTimeService == null)
            {
                throw new Exception("IDateTimeService should be set up as an instance of MockDateTimeService in testing");
            }
            dateTimeService.MockDateTime = utcNow;
        }

        public void Dispose()
        {
            _baseServiceProviderScope?.Dispose();
        }
    }
}
