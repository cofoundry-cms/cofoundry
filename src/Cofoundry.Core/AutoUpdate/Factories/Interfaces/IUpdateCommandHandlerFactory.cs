namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Factory that creates concrete implementations of IUpdateCommand handlers.
/// </summary>
public interface IUpdateCommandHandlerFactory
{
    IVersionedUpdateCommandHandler<TCommand> CreateVersionedCommand<TCommand>() where TCommand : IVersionedUpdateCommand;
    IAlwaysRunUpdateCommandHandler<TCommand> CreateAlwaysRunCommand<TCommand>() where TCommand : IAlwaysRunUpdateCommand;
}
