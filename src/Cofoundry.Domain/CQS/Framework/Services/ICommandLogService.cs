namespace Cofoundry.Domain.CQS;

/// <summary>
/// Service for logging audit data about executed commands.
/// </summary>
/// <remarks>
/// Typically only commands which implement <see cref="ILoggableCommand"/> should 
/// be logged. Commands that don't implement <see cref="ILoggableCommand"/> may 
/// bloat the log or contain sensitive data.
/// </remarks>
public interface ICommandLogService
{
    /// <summary>
    /// Called when a command has been executed successfully.
    /// </summary>
    /// <typeparam name="TCommand">The type of command that was executed, which may or may not implement <see cref="ILoggableCommand"/>.</typeparam>
    /// <param name="command">The command instance that was executed, which may or may not implement <see cref="ILoggableCommand"/>.</param>
    /// <param name="executionContext">The context the command was executed under.</param>
    Task LogAsync<TCommand>(TCommand command, IExecutionContext executionContext) where TCommand : ICommand;

    /// <summary>
    /// Called when an command execution failed.
    /// </summary>
    /// <typeparam name="TCommand">The type of command that was executed, which may or may not implement <see cref="ILoggableCommand"/>.</typeparam>
    /// <param name="command">The command instance that was executed, which may or may not implement <see cref="ILoggableCommand"/>.</param>
    /// <param name="executionContext">The context the command was executed under.</param>
    /// <param name="ex">The exception that generated the failure. Can be <see langword="null"/> if the failure was not due to an exception.</param>
    /// <returns></returns>
    Task LogFailedAsync<TCommand>(TCommand command, IExecutionContext executionContext, Exception? ex = null) where TCommand : ICommand;
}
