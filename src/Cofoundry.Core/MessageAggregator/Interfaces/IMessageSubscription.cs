using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.MessageAggregator
{
    public interface IMessageSubscription
    {
        bool CanDeliver<TMessage>();

        Task DeliverAsync(IServiceProvider serviceProvider, object message);
    }
}
