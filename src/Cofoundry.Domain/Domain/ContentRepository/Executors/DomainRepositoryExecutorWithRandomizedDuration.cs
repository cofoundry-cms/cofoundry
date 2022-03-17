using Cofoundry.Core.ExecutionDurationRandomizer;
using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// An <see cref="IDomainRepositoryExecutor"/> implementation that executes
/// command and queries inside an <see cref="IExecutionDurationRandomizerScope"/>,
/// extending the duration of the task.
/// </summary>
/// <inheritdoc/>
public class DomainRepositoryExecutorWithRandomizedDuration : IDomainRepositoryExecutor
{
    private readonly IDomainRepositoryExecutor _innerDomainRepositoryExecutor;
    private readonly IExecutionDurationRandomizerScopeManager _executionDurationRandomizerScopeManager;
    private readonly RandomizedExecutionDuration _duration;

    public DomainRepositoryExecutorWithRandomizedDuration(
        IDomainRepositoryExecutor innerDomainRepositoryExecutor,
        IExecutionDurationRandomizerScopeManager taskDurationRandomizerScopeManager,
        RandomizedExecutionDuration duration
        )
    {
        _innerDomainRepositoryExecutor = innerDomainRepositoryExecutor;
        _executionDurationRandomizerScopeManager = taskDurationRandomizerScopeManager;
        _duration = duration;
    }

    public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext)
    {
        await using (_executionDurationRandomizerScopeManager.Create(_duration))
        {
            await _innerDomainRepositoryExecutor.ExecuteAsync(command, executionContext);
        }
    }

    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext)
    {
        await using (_executionDurationRandomizerScopeManager.Create(_duration))
        {
            return await _innerDomainRepositoryExecutor.ExecuteAsync(query, executionContext);
        }
    }
}
