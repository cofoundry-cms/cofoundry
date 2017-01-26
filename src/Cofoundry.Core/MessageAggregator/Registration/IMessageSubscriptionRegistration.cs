using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// Any classes implementing IMessageSubscriptionRegistration will
    /// automatically be registered with Cofoundry at startup. If using
    /// a manual startup, use IMessageSubscriptionInitializer to initialize
    /// the registrations.
    /// </summary>
    public interface IMessageSubscriptionRegistration
    {
        /// <summary>
        /// Invoked at startup to register your message subscriptions
        /// </summary>
        /// <param name="config">Service for adding subscriptions to the message aggregator</param>
        void Register(IMessageSubscriptionConfig config);
    }
}
