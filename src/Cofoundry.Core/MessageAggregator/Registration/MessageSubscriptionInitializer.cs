using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    public class MessageSubscriptionInitializer : IMessageSubscriptionInitializer
    {
        private readonly IMessageSubscriptionConfig _messageSubscriptionConfig;
        private readonly IMessageSubscriptionRegistration[] _registrations;

        public MessageSubscriptionInitializer(
            IMessageSubscriptionConfig messageSubscriptionConfig,
            IMessageSubscriptionRegistration[] registrations
            )
        {
            _messageSubscriptionConfig = messageSubscriptionConfig;
            _registrations = registrations;
        }

        /// <summary>
        /// Adds any bootstrappable message subscriptions to the registered IMessageAggregator
        /// instance.
        /// </summary>
        public void Initialize()
        {
            foreach (var registration in _registrations)
            {
                registration.Register(_messageSubscriptionConfig);
            }
        }
    }
}
