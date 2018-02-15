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
                .RegisterSingleton<IMessageAggregatorState>(new MessageAggregatorState())
                .Register<IMessageSubscriptionInitializer, MessageSubscriptionInitializer>()
                .Register<IMessageAggregator, MessageAggregator>()
                .Register<IMessageSubscriptionConfig, MessageSubscriptionConfig>()
                .RegisterAll<IMessageSubscriptionRegistration>()
                .RegisterAllGenericImplementations(typeof(IMessageHandler<>))
                ;
        }
    }
}
