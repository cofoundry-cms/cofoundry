using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.MessageAggregator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Integration.Mocks
{
    /// <summary>
    /// A wrapper around <see cref="MessageAggregator"/> that audits
    /// messages sent to it during it's liftime.
    /// </summary>
    public class AuditableMessageAggregator : IMessageAggregator
    {
        private readonly List<object> Messages = new List<object>();
        private readonly MessageAggregator _messageAggregator;


        public AuditableMessageAggregator(
            IMessageAggregatorState state,
            IServiceProvider serviceProvider
            )
        {
            _messageAggregator = new MessageAggregator(state, serviceProvider);
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
            return Messages
                .Where(m => m is TMessage)
                .Cast<TMessage>()
                .Where(predicate)
                .Count();
        }

        public Task PublishAsync<TMessage>(TMessage message) where TMessage : class
        {
            Messages.Add(message);
            return _messageAggregator.PublishAsync(message);
        }

        public Task PublishBatchAsync<TMessage>(IReadOnlyCollection<TMessage> messages) where TMessage : class
        {
            Messages.AddRange(messages);
            return _messageAggregator.PublishBatchAsync(messages);
        }

        public void Subscribe<TMessage, TMessageHandler>()
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>
        {
            _messageAggregator.Subscribe<TMessage, TMessageHandler>();
        }
    }
}
