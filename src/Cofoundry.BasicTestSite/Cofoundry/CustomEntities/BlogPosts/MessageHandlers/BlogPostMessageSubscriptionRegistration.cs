using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class BlogPostMessageSubscriptionRegistration : IMessageSubscriptionRegistration
    {
        public void Register(IMessageSubscriptionConfig config)
        {
            config.Subscribe<ICustomEntityContentUpdatedMessage, BlogPostUpdatedMessageHandler>();
        }
    }
}
