using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Cofoundry.Core.MessageAggregator.Internal;

/// <summary>
/// Represents an individual subscription to a message
/// </summary>
/// <typeparam name="TMessageSubscribedTo">Type of message subscribed to</typeparam>
/// <typeparam name="TMessageHandler">Type of handler to invoke when a message is published</typeparam>
public class MessageSubscription<TMessageSubscribedTo, TMessageHandler>
    : IMessageSubscription
    where TMessageSubscribedTo : class
    where TMessageHandler : IMessageHandler<TMessageSubscribedTo>
{
    public bool CanDeliver<TMessage>()
    {
        return typeof(TMessageSubscribedTo).GetTypeInfo().IsAssignableFrom(typeof(TMessage));
    }

    public async Task DeliverAsync(IServiceProvider serviceProvider, object message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!(message is TMessageSubscribedTo))
        {
            throw new ArgumentException($"{nameof(message)} must be of type '{typeof(TMessageSubscribedTo).FullName}'");
        }

        var handler = serviceProvider.GetRequiredService<TMessageHandler>();
        await handler.HandleAsync((TMessageSubscribedTo)message);
    }
}
