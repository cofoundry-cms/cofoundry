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
    public class MessageAggregatorStartupConfigurationTask : IStartupConfigurationTask
    {
        private readonly IMessageSubscriptionInitializer _messageSubscriptionInitializer;

        public MessageAggregatorStartupConfigurationTask(
            IMessageSubscriptionInitializer messageSubscriptionInitializer
            )
        {
            _messageSubscriptionInitializer = messageSubscriptionInitializer;
        }

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Configure(IApplicationBuilder app)
        {
            _messageSubscriptionInitializer.Initialize();
        }
    }
}