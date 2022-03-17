namespace Cofoundry.Domain;

/// <summary>
/// Exception thrown from the domain layer when a user is prevented from
/// logging in because they have the <see cref="Data.User.RequirePasswordChange"/> 
/// flag set to <see langword="true"/>. This exception should be caught by the GUI 
/// layer and handled accordingly (i.e. redirected to password change form) before 
/// allowing the user to log in.
/// </summary>
public class PasswordChangeRequiredException : ValidationErrorException
{
    public PasswordChangeRequiredException()
        : base(UserValidationErrors.Authentication.PasswordChangeRequired.Create())
    {
    }

    public PasswordChangeRequiredException(string message)
        : base(UserValidationErrors.Authentication.PasswordChangeRequired.Customize().WithMessage(message).Create())
    {
    }

    public PasswordChangeRequiredException(ValidationError error)
        : base(error)
    {
    }
}
