using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;

namespace Cofoundry.BasicTestSite;

public class BlogPostUpdatedMessageHandler : IMessageHandler<ICustomEntityContentUpdatedMessage>
{
    private readonly ICommandExecutor _commandExecutor;

    public BlogPostUpdatedMessageHandler(
        ICommandExecutor commandExecutor
        )
    {
        _commandExecutor = commandExecutor;
    }

    public Task HandleAsync(ICustomEntityContentUpdatedMessage message)
    {
        if (message.CustomEntityDefinitionCode != BlogPostCustomEntityDefinition.DefinitionCode) return Task.CompletedTask;

        // TODO: Add logic here

        return Task.CompletedTask;
    }
}
