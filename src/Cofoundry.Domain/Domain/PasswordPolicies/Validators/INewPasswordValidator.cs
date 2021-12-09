using Cofoundry.Core.Validation;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A validator that validates a single password criteria e.g. "minimum length"
    /// or "minimum number of unique characters". This validator runs synchronously
    /// and will run before any asynchronous validators. For asynchronous validation 
    /// use <see cref="IAsyncNewPasswordValidator"/>.
    /// </summary>
    public interface INewPasswordValidator : INewPasswordValidatorBase
    {
        /// <summary>
        /// Validates a new password request against the criteria of this validator.
        /// If validation was unsuccessful a <see cref="ValidationError"/> is retured;
        /// otherwise <see langword="null"/> is returned indicating success..
        /// </summary>
        /// <param name="context">
        /// A context object that contains the new password and other data about the request that may
        /// be useful for validating the password.
        /// </param>
        ValidationError Validate(INewPasswordValidationContext context);
    }
}
