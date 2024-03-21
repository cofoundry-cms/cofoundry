namespace Cofoundry.Domain.Internal;

/// <summary>
/// Service to validate the permissions of command/query handler prior to execution.
/// </summary>
public class ExecutePermissionValidationService : IExecutePermissionValidationService
{
    private readonly IPermissionValidationService _permissionValidationService;

    public ExecutePermissionValidationService(
        IPermissionValidationService permissionValidationService
        )
    {
        _permissionValidationService = permissionValidationService;
    }

    public virtual void Validate<TCommand>(TCommand command, ICommandHandler<TCommand> commandHandler, IExecutionContext executionContext) where TCommand : ICommand
    {
        ValidateCommmandImplementation<TCommand>(commandHandler);

        if (commandHandler is IPermissionRestrictedCommandHandler<TCommand> permissionRestrictredHandler)
        {
            var permissions = permissionRestrictredHandler.GetPermissions(command);
            _permissionValidationService.EnforcePermission(permissions, executionContext.UserContext);
        }

        CheckAdditionalPermissionHandlers(commandHandler, executionContext, _permissionValidationService);
    }

    public virtual void Validate<TQuery, TResult>(TQuery query, IQueryHandler<TQuery, TResult> queryHandler, IExecutionContext executionContext) where TQuery : IQuery<TResult>
    {
        ValidateQueryImplementation<TQuery, TResult>(queryHandler);

        if (queryHandler is IPermissionRestrictedQueryHandler<TQuery, TResult> permissionRestrictedHandler)
        {
            var permissions = permissionRestrictedHandler.GetPermissions(query);
            _permissionValidationService.EnforcePermission(permissions, executionContext.UserContext);
        }

        CheckAdditionalPermissionHandlers(queryHandler, executionContext, _permissionValidationService);
    }

    protected void ValidateCommmandImplementation<TCommand>(object handler)
        where TCommand : ICommand
    {
        if (handler is IPermissionRestrictedCommandHandler)
        {
            // Check for invalid implementation
            if (!(handler is IPermissionRestrictedCommandHandler<TCommand>))
            {
                var msg = $"Invalid implementation: {handler.GetType().FullName} imeplements IPermissionRestrictedCommandHandler but not IPermissionRestrictedCommandHandler<{typeof(TCommand).FullName}>";
                throw new InvalidOperationException(msg);
            }
        }
        else
        {
            ValidateIgnorePermissionImplementation(handler);
        }
    }

    protected void ValidateQueryImplementation<TQuery, TResult>(object handler)
        where TQuery : IQuery<TResult>
    {
        if (handler is IPermissionRestrictedQueryHandler)
        {
            // Check for invalid implementation
            if (!(handler is IPermissionRestrictedQueryHandler<TQuery, TResult>))
            {
                var msg = $"Invalid implementation: {handler.GetType().FullName} imeplements IPermissionRestrictedCommandHandler but not IPermissionRestrictedCommandHandler<{typeof(TQuery).FullName}>";
                throw new InvalidOperationException(msg);
            }
        }
        else
        {
            ValidateIgnorePermissionImplementation(handler);
        }
    }

    protected void CheckAdditionalPermissionHandlers<TCommandHandler>(TCommandHandler _commandHandler, IExecutionContext executionContext, IPermissionValidationService _permissionValidationService)
    {
        // Hardcoded checking of a few additional handlers, but could use DI here to make this more flexible.
        if (_commandHandler is ISignedInPermissionCheckHandler)
        {
            _permissionValidationService.EnforceIsSignedIn(executionContext.UserContext);
        }

        if (_commandHandler is ICofoundryUserPermissionCheckHandler)
        {
            _permissionValidationService.EnforceHasPermissionToUserArea(CofoundryAdminUserArea.Code, executionContext.UserContext);
        }
    }

    /// <summary>
    /// Here we check that IIgnorePermissionCheckHandler has been implemented if no other
    /// permission handling is implemented, this ensures that the implementer has thought 
    /// about permission handling and hasn't just forgotton to add it.
    /// </summary>
    protected void ValidateIgnorePermissionImplementation(object handler)
    {
        if (!(handler is IPermissionCheckHandler))
        {
            var msg = $"{handler.GetType().FullName} does not implement an IPermissionCheckHandler. Either implement one of the permission checking interfaces or use IIgnorePermissionCheckHandler if no permission handling is required.";
            throw new InvalidOperationException(msg);
        }
    }
}
