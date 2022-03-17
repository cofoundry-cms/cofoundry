namespace Cofoundry.Core;

/// <summary>
/// Thrown when a resource is attempted to be accessed by a user who does not have 
/// sufficient permissions to do so. Can be caught furthur up the chain and handled accordingly.
/// </summary>
public class NotPermittedException : Exception
{
    private const string DEFAULT_MESSAGE = "The action is not permitted.";

    /// <summary>
    /// Initializes a new instance of the <see cref="NotPermittedException"/>
    /// class using the default error message.
    /// </summary>
    public NotPermittedException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotPermittedException"/>
    /// class using a specified error message.
    /// </summary>
    /// <param name="message">A specified message that states the error.</param>
    public NotPermittedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotPermittedException"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">A specified message that states the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a 
    /// <see langword="null"/> reference if no inner exception is specified.
    /// </param>
    public NotPermittedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
