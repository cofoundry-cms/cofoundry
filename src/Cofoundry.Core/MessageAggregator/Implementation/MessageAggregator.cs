using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// A simple Message Aggregator (Event Bus) implementation to allow subscribable message communication.
    /// </summary>
    public class MessageAggregator : IMessageAggregator
    {
        private readonly IMessageAggregatorState _state;
        private readonly IResolutionContext _resolutionContext;

        public MessageAggregator(
            IMessageAggregatorState state,
            IResolutionContext resolutionContext
            )
        {
            _state = state;
            _resolutionContext = resolutionContext;
        }

        public async Task PublishBatchAsync<TMessage>(IEnumerable<TMessage> messages) where TMessage : class
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
                        await subscription.DeliverAsync(_resolutionContext, message);
                    }
                }
            }
        }

        public async Task PublishAsync<TMessage>(TMessage message) where TMessage : class
        {
            var subs = _state.GetSubscriptionsFor<TMessage>();

            foreach (var subscription in subs)
            {
                await subscription.DeliverAsync(_resolutionContext, message);
            }
        }

        public void Subscribe<TMessage, TMessageHandler>() 
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>
        {
            _state.Subscribe(new MessageSubscription<TMessage, TMessageHandler>());
        }
    }
}
