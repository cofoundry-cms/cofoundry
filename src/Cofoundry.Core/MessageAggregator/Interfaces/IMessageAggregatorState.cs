using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// State object to keep track of subscriptions to a message aggregator
    /// </summary>
    public interface IMessageAggregatorState
    {
        /// <summary>
        /// Gets a collection of subscriptions for the specified message
        /// </summary>
        /// <typeparam name="TMessage">Type of message to get</typeparam>
        IEnumerable<IMessageSubscription> GetSubscriptionsFor<TMessage>();

        /// <summary>
        /// Adds a new message subscription to the state
        /// </summary>
        /// <param name="subscription">the message subscription to add to the state</param>
        void Subscribe(IMessageSubscription subscription);
    }
}
