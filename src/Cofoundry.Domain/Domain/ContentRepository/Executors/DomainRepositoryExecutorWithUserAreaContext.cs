using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// An <see cref="IDomainRepositoryExecutor"/> implementation that executes
/// command and queries using the user context associated with the specified
/// user area, which may be different from the ambient user area auth scheme.
/// </summary>
/// <inheritdoc/>
public class DomainRepositoryExecutorWithUserAreaContext : IDomainRepositoryExecutor
{
    private readonly IUserContextService _userContextService;
    private readonly IExecutionContextFactory _executionContextFactory;
    private readonly IDomainRepositoryExecutor _innerDomainRepositoryExecutor;
    private readonly IUserAreaDefinition _userArea;

    public DomainRepositoryExecutorWithUserAreaContext(
        IDomainRepositoryExecutor innerDomainRepositoryExecutor,
        IUserAreaDefinition userArea,
        IUserContextService userContextService,
        IExecutionContextFactory executionContextFactory
        )
    {
        _userContextService = userContextService;
        _executionContextFactory = executionContextFactory;
        _innerDomainRepositoryExecutor = innerDomainRepositoryExecutor;
        _userArea = userArea;
    }

    public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext)
    {
        var userContext = await _userContextService.GetCurrentContextByUserAreaAsync(_userArea.UserAreaCode);
        var newExecutionContext = _executionContextFactory.Create(userContext, executionContext);
        await _innerDomainRepositoryExecutor.ExecuteAsync(command, newExecutionContext);
    }

    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext)
    {
        var userContext = await _userContextService.GetCurrentContextByUserAreaAsync(_userArea.UserAreaCode);
        var newExecutionContext = _executionContextFactory.Create(userContext, executionContext);

        return await _innerDomainRepositoryExecutor.ExecuteAsync(query, newExecutionContext);
    }
}
