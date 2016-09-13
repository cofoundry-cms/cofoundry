using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.MessageAggregator
{
    public class MessageAggregatorDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterInstance<IMessageAggregatorState>(new MessageAggregatorState())
                .RegisterType<IMessageSubscriptionInitializer, MessageSubscriptionInitializer>()
                .RegisterType<IMessageAggregator, MessageAggregator>()
                .RegisterType<IMessageSubscriptionConfig, MessageSubscriptionConfig>()
                .RegisterAll<IMessageSubscriptionRegistration>()
                .RegisterAllGenericImplementations(typeof(IMessageHandler<>))
                ;
        }
    }
}
