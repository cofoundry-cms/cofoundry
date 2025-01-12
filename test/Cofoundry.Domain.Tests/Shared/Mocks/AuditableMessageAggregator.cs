﻿using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.MessageAggregator.Internal;

namespace Cofoundry.Domain.Tests.Mocks;

/// <summary>
/// A wrapper around <see cref="MessageAggregator"/> that audits
/// messages sent to it during it's liftime.
/// </summary>
public class AuditableMessageAggregator : IMessageAggregator
{
    private readonly List<object> _messages = [];
    private readonly MessageAggregator _messageAggregator;

    public AuditableMessageAggregator(
        IMessageAggregatorState state,
        IServiceProvider serviceProvider
        )
    {
        _messageAggregator = new MessageAggregator(state, serviceProvider);
    }

    /// <summary>
    /// Returns the number of messages that have been published to this instance
    /// that matches the specified type. Use this to check to check
    /// that the expected message is published only once.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to look for.</typeparam>
    /// <returns>The number of messages of the specified type.</returns>
    public int CountMessagesPublished<TMessage>()
    {
        return _messages
            .Where(m => m is TMessage)
            .Cast<TMessage>()
            .Count();
    }

    /// <summary>
    /// Returns the number of messages that have been published to this instance
    /// that matches the <paramref name="predicate"/>. Use this to check to check
    /// that the expected message is published only once.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to look for.</typeparam>
    /// <param name="predicate">An expression to filter messages by.</param>
    /// <returns>The number of messages matched by the <paramref name="predicate"/>.</returns>
    public int CountMessagesPublished<TMessage>(Func<TMessage, bool> predicate)
    {
        return _messages
            .Where(m => m is TMessage)
            .Cast<TMessage>()
            .Where(predicate)
            .Count();
    }

    public Task PublishAsync<TMessage>(TMessage message) where TMessage : class
    {
        _messages.Add(message);
        return _messageAggregator.PublishAsync(message);
    }

    public Task PublishBatchAsync<TMessage>(IReadOnlyCollection<TMessage> messages) where TMessage : class
    {
        _messages.AddRange(messages);
        return _messageAggregator.PublishBatchAsync(messages);
    }

    public void Subscribe<TMessage, TMessageHandler>()
        where TMessage : class
        where TMessageHandler : IMessageHandler<TMessage>
    {
        _messageAggregator.Subscribe<TMessage, TMessageHandler>();
    }
}
