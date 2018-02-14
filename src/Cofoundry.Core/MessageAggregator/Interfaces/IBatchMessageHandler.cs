using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// A message handler can work with a batch of messages of the same type 
    /// all at once. Use this over IMessageHandler when you're able to optimize
    /// handling a messages in batch.
    /// </summary>
    /// <typeparam name="TMessage">Type of message that this handler can process (could be an interface rather than concrete type)</typeparam>
    public interface IBatchMessageHandler<TMessage> : IMessageHandler<TMessage>
    {
        /// <summary>
        /// Method to invoke when a batch of messages of type TMessage are published
        /// </summary>
        /// <param name="message">Messages to handle</param>
        Task HandleBatchAsync(IReadOnlyCollection<TMessage> message);
    }
}
