using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation;

public class RegisterPageTemplatesAndPageBlockTypesCommandHandler : IAsyncAlwaysRunUpdateCommandHandler<RegisterPageTemplatesAndPageBlockTypesCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IExecutionContextFactory _executionContextFactory;

    public RegisterPageTemplatesAndPageBlockTypesCommandHandler(
        ICommandExecutor commandExecutor,
        IExecutionContextFactory executionContextFactory
        )
    {
        _commandExecutor = commandExecutor;
        _executionContextFactory = executionContextFactory;
    }


    public async Task ExecuteAsync(RegisterPageTemplatesAndPageBlockTypesCommand command)
    {
        var cx = await _executionContextFactory.CreateSystemUserExecutionContextAsync();
        await _commandExecutor.ExecuteAsync(new RegisterPageTemplatesCommand(), cx);
        await _commandExecutor.ExecuteAsync(new RegisterPageBlockTypesCommand(), cx);
    }
}
