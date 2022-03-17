namespace Cofoundry.Domain.Extendable;

/// <inheritdoc/>
public class DomainRepositoryExecutor : IDomainRepositoryExecutor
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;

    public DomainRepositoryExecutor(
        IQueryExecutor queryExecutor,
        ICommandExecutor commandExecutor
        )
    {
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
    }

    public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext)
    {
        await _commandExecutor.ExecuteAsync(command, executionContext);
    }

    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext)
    {
        return await _queryExecutor.ExecuteAsync(query, executionContext);
    }
}
