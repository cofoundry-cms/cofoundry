namespace Cofoundry.Domain;

/// <summary>
/// Exception thrown when an attempt to access a page is made, but the user
/// account does not meet the access rule criteria.
/// </summary>
public class AccessRuleViolationException : NotPermittedException
{
    private const string DEFAULT_MESSAGE = "Access to the resource is not permitted.";

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessRuleViolationException"/>
    /// class using the default error message.
    /// </summary>
    public AccessRuleViolationException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotPermittedException"/>
    /// class using a specified error message.
    /// </summary>
    /// <param name="message">A specified message that states the error.</param>
    public AccessRuleViolationException(string message)
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
    public AccessRuleViolationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
