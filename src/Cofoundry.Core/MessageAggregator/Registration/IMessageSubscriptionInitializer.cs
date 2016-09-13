using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    public interface IMessageSubscriptionInitializer
    {
        /// <summary>
        /// Adds any bootstrappable message subscriptions to the registered IMessageAggregator
        /// instance.
        /// </summary>
        void Initialize();
    }
}
