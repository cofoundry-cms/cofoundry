using Cofoundry.Core.Time;

namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// Factory for creating an IExecutionContext instance.
/// </summary>
public class ExecutionContextFactory : IExecutionContextFactory
{
    private readonly IUserContextService _userContextService;
    private readonly IDateTimeService _dateTimeService;

    public ExecutionContextFactory(
        IUserContextService userContextService,
        IDateTimeService dateTimeService
        )
    {
        _userContextService = userContextService;
        _dateTimeService = dateTimeService;
    }


    /// <inheritdoc />
    public async Task<IExecutionContext> CreateAsync()
    {
        var userContext = await _userContextService.GetCurrentContextAsync();
        return Create(userContext);
    }


    /// <inheritdoc />
    public IExecutionContext Create(IUserContext userContext, IExecutionContext? executionContextToCopy = null)
    {
        ArgumentNullException.ThrowIfNull(userContext);

        var newContext = new ExecutionContext();
        newContext.UserContext = userContext;

        if (executionContextToCopy != null)
        {
            newContext.ExecutionDate = executionContextToCopy.ExecutionDate;
        }
        else
        {
            newContext.ExecutionDate = _dateTimeService.UtcNow();
        }

        return newContext;
    }

    /// <inheritdoc />
    public async Task<IExecutionContext> CreateSystemUserExecutionContextAsync(IExecutionContext? executionContextToCopy = null)
    {
        var userContext = await _userContextService.GetSystemUserContextAsync();
        return Create(userContext, executionContextToCopy);
    }
}