using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    /// <summary>
    /// Used to run all instances of IMessageSubscriptionRegistration registered
    /// with the DI container. Typically run when Cofoundry startsup, but you can
    /// run this manually if you're not using the Cofoundry startup process.
    /// </summary>
    public interface IMessageSubscriptionInitializer
    {
        /// <summary>
        /// Adds any bootstrappable message subscriptions to the registered IMessageAggregator
        /// instance.
        /// </summary>
        void Initialize();
    }
}
