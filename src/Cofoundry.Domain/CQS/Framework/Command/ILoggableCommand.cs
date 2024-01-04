namespace Cofoundry.Domain.CQS;

/// <summary>
/// Indicates that the execution of an <see cref="ICommand"/> 
/// can be logged to an audit logging service.
/// </summary>
public interface ILoggableCommand : ICommand
{
}
