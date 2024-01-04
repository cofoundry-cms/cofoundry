namespace Cofoundry.Domain.CQS;

/// <summary>
/// An exception for when the <see cref="ExecutionContext"/> supplied to a handler 
/// is not valid because data has not been initialized correctly.
/// </summary>
public class ExecutionContextNotInitializedException : Exception
{
    private const string DEFAULT_MESSAGE = "The ExecutionContext was not initialized correctly.";

    public ExecutionContextNotInitializedException()
        : base(DEFAULT_MESSAGE)
    {
    }

    public ExecutionContextNotInitializedException(string message)
        : base(message)
    {
    }
}
