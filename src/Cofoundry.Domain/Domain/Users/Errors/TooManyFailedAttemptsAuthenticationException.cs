using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the domain layer when an authentication
    /// attempt is invalid because too many invalid attempts have been logged.
    /// </summary>
    public class TooManyFailedAttemptsAuthenticationException : ValidationErrorException
    {
        public TooManyFailedAttemptsAuthenticationException()
            : base(UserValidationErrors.Authentication.TooManyFailedAttempts.Create())
        {
        }

        public TooManyFailedAttemptsAuthenticationException(string message)
            : base(UserValidationErrors.Authentication.TooManyFailedAttempts.Customize().WithMessage(message).Create())
        {
        }

        public TooManyFailedAttemptsAuthenticationException(ValidationError error)
            : base(error)
        {
        }
    }
}
