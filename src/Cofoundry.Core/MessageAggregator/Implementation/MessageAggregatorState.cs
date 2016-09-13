using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    public class MessageAggregatorState : IMessageAggregatorState
    {
        private readonly ConcurrentBag<IMessageSubscription> _subscriptions = new ConcurrentBag<IMessageSubscription>();

        public IEnumerable<IMessageSubscription> GetSubscriptionsFor<TMessage>()
        {
            return _subscriptions
                .Where(s => s.CanDeliver<TMessage>());
        }

        public void Subscribe(IMessageSubscription subscription)
        {
            _subscriptions.Add(subscription);
        }
    }
}
