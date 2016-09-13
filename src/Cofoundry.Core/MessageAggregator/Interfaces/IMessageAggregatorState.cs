using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    public interface IMessageAggregatorState
    {
        IEnumerable<IMessageSubscription> GetSubscriptionsFor<TMessage>();
        void Subscribe(IMessageSubscription subscription);
    }
}
