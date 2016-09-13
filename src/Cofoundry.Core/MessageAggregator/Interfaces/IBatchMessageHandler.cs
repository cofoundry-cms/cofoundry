using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// Indicates that a handler can handle a batch of messages of the same type
    /// </summary>
    /// <typeparam name="TMessage">Type of message that this handler can process (could be an interface rather than concrete type)</typeparam>
    public interface IBatchMessageHandler<TMessage> : IMessageHandler<TMessage>
    {
        Task HandleBatchAsync(IEnumerable<TMessage> message);
    }
}
