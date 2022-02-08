using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the domain layer when an authentication 
    /// attempt is invalid because the username or password is invalid.
    /// </summary>
    public class InvalidCredentialsAuthenticationException : ValidationErrorException
    {
        public InvalidCredentialsAuthenticationException()
            : base(UserValidationErrors.Authentication.InvalidCredentials.Create())
        {
        }

        public InvalidCredentialsAuthenticationException(string message)
            : base(UserValidationErrors.Authentication.InvalidCredentials.Customize().WithMessage(message).Create())
        {
        }

        public InvalidCredentialsAuthenticationException(ValidationError error)
            : base(error)
        {
        }
    }
}
