using Cofoundry.Core.BackgroundTasks;
using Cofoundry.Domain.CQS;

namespace HangfireSample.Cofoundry.BackgroundTasks;

public class ProductAddingBackgroundTask : IAsyncRecurringBackgroundTask
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IExecutionContextFactory _executionContextFactory;

    public ProductAddingBackgroundTask(
        ICommandExecutor commandExecutor,
        IExecutionContextFactory executionContextFactory
        )
    {
        _commandExecutor = commandExecutor;
        _executionContextFactory = executionContextFactory;
    }

    public async Task ExecuteAsync()
    {
        var title = Guid.NewGuid();
        var command = new AddCustomEntityCommand()
        {
            CustomEntityDefinitionCode = ProductCustomEntityDefinition.DefinitionCode,
            Title = title.ToString(),
            Model = new ProductDataModel() { Description = "Description of " + title },
            Publish = true
        };

        var elevatedExecutionContext = await _executionContextFactory.CreateSystemUserExecutionContextAsync();

        await _commandExecutor.ExecuteAsync(command, elevatedExecutionContext);
    }
}
