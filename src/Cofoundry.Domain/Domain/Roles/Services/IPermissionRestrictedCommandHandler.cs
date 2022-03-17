namespace Cofoundry.Domain;

/// <summary>
/// Used to ensure a user has a specific set of permissions before the handler is executed
/// </summary>
public interface IPermissionRestrictedCommandHandler : IPermissionCheckHandler
{
}

/// <summary>
/// Used to ensure a user has a specific set of permissions before the handler is executed
/// </summary>
public interface IPermissionRestrictedCommandHandler<TCommand> : IPermissionRestrictedCommandHandler
    where TCommand : ICommand
{
    IEnumerable<IPermissionApplication> GetPermissions(TCommand command);
}
