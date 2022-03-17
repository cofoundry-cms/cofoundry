namespace Cofoundry.Domain.CQS;

/// <summary>
/// Handles the execution of a <see cref="TCommand"/> instance.
/// </summary>
/// <typeparam name="TCommand">Type of ICommand to handle.</typeparam>
public interface ICommandHandler<TCommand>
     where TCommand : ICommand
{
    /// <summary>
    /// Executes the specified command using the specified <see cref="IExecutionContext"/>.
    /// </summary>
    /// <param name="command"><see cref="ICommand"/> instance containing the parameters of the command.</param>
    /// <param name="executionContext">The context to execute the command under, i.e. which user and execution timestamp etc.</param>
    Task ExecuteAsync(TCommand command, IExecutionContext executionContext);
}
