using Cofoundry.Core.Validation;
using Cofoundry.Domain.Internal;
using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Validates that a password does not match either their username
    /// or email address, based on a case-insenstivie equality comparison.
    /// </summary>
    /// <inheritdoc/>
    public class NotBePersonalDataNewPasswordValidator : INewPasswordValidator
    {
        private static string ERROR_CODE = NewPasswordValidationErrorCodes.AddNamespace("not-personal-data");

        public string Criteria => $"Must not be your email or username.";

        public ValidationError Validate(INewPasswordValidationContext context)
        {
            if (Matches(context.Password, context.Email))
            {
                return new ValidationError()
                {
                    ErrorCode = ERROR_CODE + "-email",
                    Message = $"Password cannot be your email.",
                    Properties = new string[] { context.PropertyName }
                };
            }

            if (Matches(context.Password, context.Username))
            {
                return new ValidationError()
                {
                    ErrorCode = ERROR_CODE + "-username",
                    Message = $"Password cannot be your username.",
                    Properties = new string[] { context.PropertyName }
                };
            }

            return null;
        }

        private static bool Matches(string data, string password)
        {
            return !string.IsNullOrEmpty(data)
                && data.Equals(password, StringComparison.OrdinalIgnoreCase);
        }
    }
}
