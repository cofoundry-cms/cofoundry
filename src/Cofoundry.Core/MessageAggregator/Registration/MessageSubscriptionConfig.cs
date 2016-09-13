using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    public class MessageSubscriptionConfig : IMessageSubscriptionConfig
    {
        private readonly IMessageAggregator _messageAggregator;

        public MessageSubscriptionConfig(
            IMessageAggregator messageAggregator
            )
        {
            _messageAggregator = messageAggregator;
        }

        public void Subscribe<TMessage, TMessageHandler>()
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>
        {
            _messageAggregator.Subscribe<TMessage, TMessageHandler>();
        }
    }
}
