namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// Factory to create ICommandHandler instances. This factory allows you to override
/// or wrap the existing ICommandHandler implementation
/// </summary>
public interface ICommandHandlerFactory
{
    /// <summary>
    /// Creates a new ICommandHandler instance with the specified type signature.
    /// </summary>
    ICommandHandler<T> CreateAsyncHandler<T>() where T : ICommand;
}
