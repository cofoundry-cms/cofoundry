namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Base interface for IVersionedUpdateCommand handlers
/// </summary>
/// <typeparam name="TCommand">Type of command to execute</typeparam>
public interface IVersionedUpdateCommandHandler<in TCommand> where TCommand : IVersionedUpdateCommand
{
}
