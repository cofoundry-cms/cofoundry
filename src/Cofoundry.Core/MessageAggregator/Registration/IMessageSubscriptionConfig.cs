using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    public interface IMessageSubscriptionConfig
    {
        void Subscribe<TMessage, TMessageHandler>()
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>;
    }
}
