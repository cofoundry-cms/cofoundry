namespace Cofoundry.Core.MessageAggregator;

public interface IMessageSubscription
{
    bool CanDeliver<TMessage>();

    Task DeliverAsync(IServiceProvider serviceProvider, object message);
}
