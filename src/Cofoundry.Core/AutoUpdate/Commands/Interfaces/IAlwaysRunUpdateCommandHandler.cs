namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Base interface for IAlwaysRunUpdateCommand handlers
/// </summary>
/// <typeparam name="TCommand">Type of command to execute</typeparam>
public interface IAlwaysRunUpdateCommandHandler<in TCommand> where TCommand : IAlwaysRunUpdateCommand
{
}
