using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// An <see cref="IDomainRepositoryExecutor"/> implementation that executes
/// command and queries using a specific execution context.
/// </summary>
/// <inheritdoc/>
public class DomainRepositoryExecutorWithExecutionContext : IDomainRepositoryExecutor
{
    private readonly IDomainRepositoryExecutor _innerDomainRepositoryExecutor;
    private readonly IExecutionContext _executionContextOverride;

    public DomainRepositoryExecutorWithExecutionContext(
        IDomainRepositoryExecutor innerDomainRepositoryExecutor,
        IExecutionContext executionContext
        )
    {
        _innerDomainRepositoryExecutor = innerDomainRepositoryExecutor;
        _executionContextOverride = executionContext;
    }

    public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext)
    {
        await _innerDomainRepositoryExecutor.ExecuteAsync(command, _executionContextOverride);
    }

    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext)
    {
        return await _innerDomainRepositoryExecutor.ExecuteAsync(query, _executionContextOverride);
    }
}
