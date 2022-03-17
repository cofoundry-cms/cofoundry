namespace Cofoundry.Domain.CQS;

/// <summary>
/// Indicates that the execution of an ICommand should be logged.
/// </summary>
public interface ILoggableCommand : ICommand
{
}
