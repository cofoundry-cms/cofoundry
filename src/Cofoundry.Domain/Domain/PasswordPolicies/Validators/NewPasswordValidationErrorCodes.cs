using Cofoundry.Core;
using Cofoundry.Core.Validation.Internal;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to namespace Cofoundry error codes for <see cref="INewPasswordValidator"/>
    /// errors e.g. "cf-new-password-max-length-exceeded".
    /// </summary>
    public class NewPasswordValidationErrorCodes
    {
        /// <summary>
        /// Namespaces a validation error code for an <see cref="INewPasswordValidator"/>
        /// into the format "cf-new-password-{error-code}".
        /// </summary>
        /// <param name="errorCode">
        /// The code that succintly describes the error which should be in slug
        /// format e.g. "max-length-exceeded".
        /// </param>
        public static string AddNamespace(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode)) throw new ArgumentEmptyException();

            return ValidationErrorCodes.AddNamespace(errorCode, "new-password");
        }
    }
}