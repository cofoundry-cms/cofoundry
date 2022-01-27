using Cofoundry.Core.Validation;
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
        public string Criteria => $"Must not be your email or username.";

        public ValidationError Validate(INewPasswordValidationContext context)
        {
            if (Matches(context.Password, context.Email))
            {
                return PasswordPolicyValidationErrors.NotPersonalData.Email.Create(context.PropertyName);
            }

            if (Matches(context.Password, context.Username))
            {
                return PasswordPolicyValidationErrors.NotPersonalData.Username.Create(context.PropertyName);
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
