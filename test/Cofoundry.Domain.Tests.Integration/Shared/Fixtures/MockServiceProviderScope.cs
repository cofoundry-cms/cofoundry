using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Time;
using Cofoundry.Core.Time.Mocks;
using Cofoundry.Domain.Tests.Shared.Mocks;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Domain.Tests.Integration
{
    public class MockServiceProviderScope : IServiceProvider, IDisposable
    {
        private readonly IServiceScope _baseServiceProviderScope;

        public MockServiceProviderScope(
            IServiceProvider baseServiceProvider
            )
        {
            _baseServiceProviderScope = baseServiceProvider.CreateScope();
        }

        public object GetService(Type serviceType)
        {
            return _baseServiceProviderScope.ServiceProvider.GetService(serviceType);
        }

        public void MockDateTime(DateTime utcNow)
        {
            var dateTimeService = _baseServiceProviderScope.ServiceProvider.GetService<IDateTimeService>() as MockDateTimeService;
            if (dateTimeService == null)
            {
                throw new Exception($"{nameof(IDateTimeService)} is expected to be an instance of {nameof(MockDateTimeService)} in testing");
            }
            dateTimeService.MockDateTime = utcNow;
        }

        /// <summary>
        /// Returns the number of messages that have been published to this instance
        /// that matches the <paramref name="predicate"/>. Use this to check to check
        /// that the expected message is published only once.
        /// </summary>
        /// <typeparam name="TMessage">Type of message to look for.</typeparam>
        /// <param name="expression">An expression to filter messages by.</param>
        /// <returns>The number of messages matched by the <paramref name="predicate"/>.</returns>
        public int CountMessagesPublished<TMessage>(Func<TMessage, bool> predicate)
        {
            var auditableMessageAggregator = _baseServiceProviderScope.ServiceProvider.GetService<IMessageAggregator>() as AuditableMessageAggregator;
            if (auditableMessageAggregator == null)
            {
                throw new Exception($"{nameof(IMessageAggregator)} is expected to be an instance of {nameof(AuditableMessageAggregator)} in testing");
            }

            return auditableMessageAggregator.CountMessagesPublished(predicate);
        }

        public void Dispose()
        {
            _baseServiceProviderScope?.Dispose();
        }
    }
}
