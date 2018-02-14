using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// A simple Message Aggregator (Event Bus) to allow subscribable message communication.
    /// </summary>
    public interface IMessageAggregator
    {
        /// <summary>
        /// Publishes the specified message, invoking any handlers subscribed to
        /// the message.
        /// </summary>
        /// <typeparam name="TMessage">Message type. This should be a simple serializable object</typeparam>
        /// <param name="message">The message to publish</param>
        Task PublishAsync<TMessage>(TMessage message) where TMessage : class;

        /// <summary>
        /// Some message handlers are optimized to act on a batch of messages, so it can be a more 
        /// performant to publish multiple messages of the same type together if you're working with
        /// bulk data.
        /// </summary>
        /// <typeparam name="TMessage">Message type. This should be a simple serializable object</typeparam>
        /// <param name="messages">Collection of messages to publish</param>
        Task PublishBatchAsync<TMessage>(IReadOnlyCollection<TMessage> messages) where TMessage : class;

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
        void Subscribe<TMessage, TMessageHandler>()
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>;
    }
}
