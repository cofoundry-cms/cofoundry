using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator.Internal
{
    /// <summary>
    /// Service for adding subscriptions to the message aggregator
    /// </summary>
    public class MessageSubscriptionConfig : IMessageSubscriptionConfig
    {
        private readonly IMessageAggregator _messageAggregator;

        public MessageSubscriptionConfig(
            IMessageAggregator messageAggregator
            )
        {
            _messageAggregator = messageAggregator;
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
        public IMessageSubscriptionConfig Subscribe<TMessage, TMessageHandler>()
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>
        {
            _messageAggregator.Subscribe<TMessage, TMessageHandler>();

            return this;
        }
    }
}
