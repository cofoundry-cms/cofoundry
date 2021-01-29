using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator.Internal
{
    /// <summary>
    /// State object to keep track of subscriptions to a message aggregator
    /// </summary>
    public class MessageAggregatorState : IMessageAggregatorState
    {
        private readonly ConcurrentBag<IMessageSubscription> _subscriptions = new ConcurrentBag<IMessageSubscription>();

        /// <summary>
        /// Gets a collection of subscriptions for the specified message
        /// </summary>
        /// <typeparam name="TMessage">Type of message to get</typeparam>
        public IEnumerable<IMessageSubscription> GetSubscriptionsFor<TMessage>()
        {
            return _subscriptions
                .Where(s => s.CanDeliver<TMessage>());
        }

        /// <summary>
        /// Adds a new message subscription to the state
        /// </summary>
        /// <param name="subscription">the message subscription to add to the state</param>
        public void Subscribe(IMessageSubscription subscription)
        {
            _subscriptions.Add(subscription);
        }
    }
}
