using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.MessageAggregator.Internal;

namespace Cofoundry.Core.MessageAggregator.Registration
{
    public class MessageAggregatorDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterSingleton<IMessageAggregatorState>(new MessageAggregatorState())
                .Register<IMessageSubscriptionInitializer, MessageSubscriptionInitializer>()
                .Register<IMessageAggregator, Internal.MessageAggregator>()
                .Register<IMessageSubscriptionConfig, MessageSubscriptionConfig>()
                .RegisterAll<IMessageSubscriptionRegistration>()
                .RegisterAllGenericImplementations(typeof(IMessageHandler<>))
                ;
        }
    }
}
