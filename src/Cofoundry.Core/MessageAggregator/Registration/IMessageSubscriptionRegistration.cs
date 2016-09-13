using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.MessageAggregator
{
    public interface IMessageSubscriptionRegistration
    {
        void Register(IMessageSubscriptionConfig config);
    }
}
