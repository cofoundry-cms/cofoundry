using Cofoundry.Core.MessageAggregator;

namespace Dev.Sandbox;

public class BlogPostMessageSubscriptionRegistration : IMessageSubscriptionRegistration
{
    public void Register(IMessageSubscriptionConfig config)
    {
        config.Subscribe<ICustomEntityContentUpdatedMessage, BlogPostUpdatedMessageHandler>();
    }
}
