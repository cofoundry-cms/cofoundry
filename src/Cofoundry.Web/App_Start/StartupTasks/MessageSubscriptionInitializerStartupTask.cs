using Cofoundry.Core.MessageAggregator;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Adds any bootstrappable message subscriptions to the registered IMessageAggregator
    /// instance.
    /// </summary>
    public class MessageSubscriptionInitializerStartupTask : IStartupTask
    {
        private readonly IMessageSubscriptionInitializer _messageSubscriptionInitializer;

        public MessageSubscriptionInitializerStartupTask(
            IMessageSubscriptionInitializer messageSubscriptionInitializer
            )
        {
            _messageSubscriptionInitializer = messageSubscriptionInitializer;
        }

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IApplicationBuilder app)
        {
            _messageSubscriptionInitializer.Initialize();
        }
    }
}