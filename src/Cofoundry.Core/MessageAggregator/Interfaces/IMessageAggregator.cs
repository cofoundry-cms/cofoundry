using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// A simple Message Aggregator (Event Bus) to allow subscribable message communication.
    /// </summary>
    public interface IMessageAggregator
    {
        Task PublishAsync<TMessage>(TMessage message) where TMessage : class;
        Task PublishBatchAsync<TMessage>(IEnumerable<TMessage> messages) where TMessage : class;

        void Subscribe<TMessage, TMessageHandler>()
            where TMessage : class
            where TMessageHandler : IMessageHandler<TMessage>;
    }
}
