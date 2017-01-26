using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// Indicates a handler that can perform an action when a message of 
    /// a certain type is received. (could be an interface rather than concrete type)
    /// </summary>
    /// <typeparam name="TMessage">Type of message that this handler can process</typeparam>
    public interface IMessageHandler<TMessage>
    {
        /// <summary>
        /// Method to invoke when a message of type TMessage is published 
        /// </summary>
        /// <param name="message">Message instance to handle</param>
        Task HandleAsync(TMessage message);
    }
}
