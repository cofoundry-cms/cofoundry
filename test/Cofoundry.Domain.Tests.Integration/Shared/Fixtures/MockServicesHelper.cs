using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Time;
using Cofoundry.Core.Time.Mocks;
using Cofoundry.Domain.Tests.Integration.Mocks;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Convenience methods to make it easier to work
    /// with mocks and service wrappers set up in the 
    /// service collection for testing.
    /// </summary>
    public class MockServicesHelper
    {
        private readonly TestApplicationServiceScope _serviceScope;

        public MockServicesHelper(
            TestApplicationServiceScope serviceScope
            )
        {
            _serviceScope = serviceScope;
        }

        /// <summary>
        /// Sets the date and time used by Cofoundry (via 
        /// <see cref="IDateTimeService"/>) to a specific value.
        /// </summary>
        /// <param name="utcNow">The UTC time to set as the current time.</param>
        public void MockDateTime(DateTime utcNow)
        {
            var dateTimeService = _serviceScope.GetService<IDateTimeService>() as MockDateTimeService;
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
            var auditableMessageAggregator = _serviceScope.GetService<IMessageAggregator>() as AuditableMessageAggregator;
            if (auditableMessageAggregator == null)
            {
                throw new Exception($"{nameof(IMessageAggregator)} is expected to be an instance of {nameof(AuditableMessageAggregator)} in testing");
            }

            return auditableMessageAggregator.CountMessagesPublished(predicate);
        }
    }
}
