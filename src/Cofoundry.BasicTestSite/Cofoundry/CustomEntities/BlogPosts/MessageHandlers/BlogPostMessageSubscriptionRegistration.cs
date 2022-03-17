using Cofoundry.Core.MessageAggregator;

namespace Cofoundry.BasicTestSite;

public class BlogPostMessageSubscriptionRegistration : IMessageSubscriptionRegistration
{
    public void Register(IMessageSubscriptionConfig config)
    {
        config.Subscribe<ICustomEntityContentUpdatedMessage, BlogPostUpdatedMessageHandler>();
    }
}
