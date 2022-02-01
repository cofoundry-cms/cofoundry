using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the domain layer when a user is prevented from
    /// logging in because their account has not been verified and the user
    /// area is configured to require account verification. This exception can be 
    /// caught in the UI layer and handled accordingly e.g. advise users and allow 
    /// them to request a new verification email.
    /// </summary>
    public class AccountNotVerifiedException : ValidationErrorException
    {
        public AccountNotVerifiedException()
            : base(UserValidationErrors.Authentication.AccountNotVerified.Create())
        {
        }

        public AccountNotVerifiedException(string message)
            : base(UserValidationErrors.Authentication.AccountNotVerified.Customize().WithMessage(message).Create())
        {
        }

        public AccountNotVerifiedException(ValidationError error)
            : base(error)
        {
        }
    }
}