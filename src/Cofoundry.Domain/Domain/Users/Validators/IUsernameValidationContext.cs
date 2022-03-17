namespace Cofoundry.Domain;

/// <summary>
/// Describes a request to validate a username for a user account, including any
/// data that might be useful during validation.
/// </summary>
public interface IUsernameValidationContext
{
    /// <summary>
    /// The name of the command property being validated, which is typically "Username". 
    /// This can be used when generating any validation errors that need to be returned. 
    /// This field is optional and so can be <see langword="null"/>.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> belonging to the user
    /// being validated.
    /// </summary>
    public string UserAreaCode { get; }

    /// <summary>
    /// The database id of the user the username belongs to. This can be <see langword="null"/> if
    /// the username is being validated for a new user.
    /// </summary>
    public int? UserId { get; }

    /// <summary>
    /// The result of the username normalization and uniquification process, which should not be null.
    /// </summary>
    public UsernameFormattingResult Username { get; }

    /// <summary>
    /// The context of the currently executing query or command.
    /// </summary>
    public IExecutionContext ExecutionContext { get; }
}
