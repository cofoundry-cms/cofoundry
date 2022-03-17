namespace Cofoundry.Domain;

/// <summary>
/// Describes a request to validate a new password for a user, including any
/// data that might be useful during validation.
/// </summary>
public interface INewPasswordValidationContext
{
    /// <summary>
    /// The name of the command property being validated, which is typically "Password" 
    /// or "NewPassword". This can be used when generating any validation errors that
    /// need to be returned. This field is optional and so can be <see langword="null"/>.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> belonging to the user
    /// being validated.
    /// </summary>
    public string UserAreaCode { get; }

    /// <summary>
    /// The database id of the user the new password belongs to. This can be <see langword="null"/> if
    /// the password is being validated for a new user.
    /// </summary>
    public int? UserId { get; }

    /// <summary>
    /// The password to validate.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// The current users password. This will be <see langword="null"/> unless the current
    /// password has been supplied to authenticate a change password request.
    /// </summary>
    public string CurrentPassword { get; }

    /// <summary>
    /// The username of the user updating their password.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// The email address of the user updating their password. This may be <see langword="null"/>
    /// if the user are does not require an email and one has not been supplied.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// The context of the currently executing query or command.
    /// </summary>
    public IExecutionContext ExecutionContext { get; }
}
