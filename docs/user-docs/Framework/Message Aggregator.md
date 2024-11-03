Cofoundry includes a simple Message Aggregator / Event Bus implementation to allow subscribable message communication.

This is particularly useful when you want to tap into Cofoundry events. An example of this is if you want to update a search index whenever an entity is saved.

## Subscribing to a Message

Cofoundry already has message classes built in for core entity types, a full list is available in the [Subscribable Messages List](/references/subscribable-messages).

#### Message Handlers

A message handler contains the code that is executed when a message is published. Handlers should inherit from `IMessageHandler<TMessage>` or from `IBatchMessageHandler<TMessage>` when your able to take advantage of batch message handling.

Here's a simple example of a handler that detects blog post custom entities being deleted and removes the entity from a search index:

```csharp
public class BlogPostDeletedMessageHandler : IMessageHandler<CustomEntityDeletedMessage>
{
    private readonly IExampleSearchIndexRepository _exampleSearchIndexRepository;

    public BlogPostDeletedMessageHandler(
        IExampleSearchIndexRepository exampleSearchIndexRepository
        )
    {
        _exampleSearchIndexRepository = exampleSearchIndexRepository;
    }

    public async Task HandleAsync(CustomEntityDeletedMessage message)
    {
        if (message.CustomEntityDefinitionCode == BlogPostCustomEntityDefinition.DefinitionCode)
        {
            await _exampleSearchIndexRepository.DeleteAsync(message.CustomEntityId);
        }
    }
}
```

#### Registering Subscriptions

The best way to set up your message subscriptions is to create a registration class that implements `IMessageSubscriptionRegistration`, doing so will ensure that Cofoundry automatically registers any message subscriptions at startup.

```csharp
using Cofoundry.Core.MessageAggregator

public class BlogPostMessageSubscriptionRegistration : IMessageSubscriptionRegistration
{
    public void Register(IMessageSubscriptionConfig config)
    {
        config.Subscribe<ICustomEntityContentUpdatedMessage, BlogPostUpdatedMessageHandler>();
        config.Subscribe<CustomEntityDeletedMessage, BlogPostDeletedMessageHandler>();
    }
}
```

## Creating a Message

Creating your own message class is simple, it should be a basic POCO and should only include properties that are serializable (the simpler the better):

```csharp
/// <summary>
/// This message is published when a page is deleted
/// </summary>
public class PageDeletedMessage
{
    /// <summary>
    /// Id of the page that has been deleted
    /// </summary>
    public int PageId { get; set; }
}
```

Interfaces can also be subscribed to so if you want to add an interface to represent several different messages you may do so. In this example the interface `IPageContentUpdatedMessage` is used to represent any message that results from updating the content of a page:

```csharp
/// <summary>
/// This message is published when page draft has been 
/// updated. To be notified when any part of a page changes 
/// (including page data and publish changes) you should 
/// subscribe to IPageContentUpdatedMessage
/// </summary>
public class PageDraftVersionUpdatedMessage: IPageContentUpdatedMessage
{
    /// <summary>
    /// Id of the page with the draft version being updated
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// Because the draft version has been modified this will
    /// always be false
    /// </summary>
    public bool HasPublishedVersionChanged => false;

    /// <summary>
    /// Id of the page version that has been updated
    /// </summary>
    public int PageVersionId { get; set; }
}
```

## Publishing a Message

Publishing a message is as simple as calling `PublishAsync` on an instance of `IMessageAggregator`. Here's an example:

```csharp
using Cofoundry.Core.MessageAggregator;

public class AddPageCommandHandler
{
    private readonly IMessageAggregator _messageAggregator;

    public AddPageCommandHandler(
        IMessageAggregator messageAggregator
        )
    {
        _messageAggregator = messageAggregator;
    }

    public async Task AddPage(AddPageCommand command)
    {
        var page = new Page();
        // ... code to add the page

        await _messageAggregator.PublishAsync(new PageAddedMessage()
        {
            PageId = page.PageId,
            HasPublishedVersionChanged = command.Publish
        });
    }
}
```

If you want to publish multiple messages of the same type, you can do so using `IMessageAggregator.PublishBatchAsync`. Some message handlers are optimized to act on a batch of messages, so this can be a more performant way of doing this. An example of how this can be used is [ReOrderCustomEntitiesCommandHandler](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Domain/Domain/CustomEntities/Commands/ReOrderCustomEntitiesCommandHandler.cs), where re-ordering can cause changes to many custom entities at a time.