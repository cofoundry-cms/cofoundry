using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.MessageAggregator.Internal
{
    /// <summary>
    /// A simple Message Aggregator (Event Bus) implementation to allow subscribable message communication.
    /// </summary>
    public class MessageAggregator : IMessageAggregator
    {
        private readonly IMessageAggregatorState _state;
        private readonly IServiceProvider _serviceProvider;

        public MessageAggregator(
            IMessageAggregatorState state,
            IServiceProvider serviceProvider
            )
        {
            _state = state;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Publishes the specified message, invoking any handlers subscribed to
        /// the message.
        /// </summary>
        /// <typeparam name="TMessage">Message type. This should be a simple serializable object</typeparam>
        /// <param name="message">The message to publish</param>
        public async Task PublishAsync<TMessage>(TMessage message) where TMessage : class
        {
            var subs = _state.GetSubscriptionsFor<TMessage>();

            foreach (var subscription in subs)
            {
                await subscription.DeliverAsync(_serviceProvider, message);
            }
        }

        /// <summary>
        /// Some message handlers are optimized to act on a batch of messages, so it can be a more 
        /// performant to publish multiple messages of the same type together if you're working with
        /// bulk data.
        /// </summary>
        /// <typeparam name="TMessage">Message type. This should be a simple serializable object</typeparam>
        /// <param name="messages">Collection of messages to publish</param>
        public async Task PublishBatchAsync<TMessage>(IReadOnlyCollection<TMessage> messages) where TMessage : class
        {
            var subs = _state.GetSubscriptionsFor<TMessage>();

            foreach (var subscription in subs)
            {
                if (subscription is IBatchMessageHandler<TMessage>)
                {
                    await ((IBatchMessageHandler<TMessage>)subscription).HandleBatchAsync(messages);
                }
                else
                {
                    foreach (var message in messages)
                    {
                        await subscription.DeliverAsync(_serviceProvider, message);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes the specified handler to the spified message type. Message types
        /// can be conrecrete types or interfaces that act as groups spanning several 
        /// messages types
        /// </summary>
        /// <typeparam name="TMessage">
        /// The type of message to subscribe to. Message types
        /// can be conrecrete types or interfaces that act as groups spanning several 
        /// messages types
        /// </typeparam>
        /// <typeparam name="TMessageHandler">The handler to invoke when the message is published</typeparam>
        public void Subscribe<TMessage, TMessageHandler>() 
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>
        {
            _state.Subscribe(new MessageSubscription<TMessage, TMessageHandler>());
        }
    }
}
