using Cofoundry.Core.Validation.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Library of validation errors relating to users.
/// </summary>
public static class PasswordPolicyValidationErrors
{
    /// <summary>
    /// The minimum number of unique characters in a password has not been met. This is to prevent
    /// repeated character sets in passwords e.g. the password "YYZ-YYZ-YYZ" contains 3 unique characters.
    /// Principally used by <see cref="MinUniqueCharactersNewPasswordValidator"/>.
    /// </summary>
    public static readonly ValidationErrorTemplate MinUniqueCharacters = new ValidationErrorTemplate(
        AddNamespace("min-unique-characters-not-met"),
        "Password must have at least {0} unique characters."
        );

    /// <summary>
    /// The password is shorter then the limit configured limit.
    /// Principally used by <see cref="MinLengthNewPasswordValidator"/>.
    /// </summary>
    public static readonly ValidationErrorTemplate MinLengthNotMet = new ValidationErrorTemplate(
        AddNamespace("min-length-not-met"),
        "Password must be at least {0} characters"
        );

    /// <summary>
    /// The password is longer then the limit configured limit.
    /// Principally used by <see cref="MaxLengthNewPasswordValidator"/>.
    /// </summary>
    public static readonly ValidationErrorTemplate MaxLengthExceeded = new ValidationErrorTemplate(
        AddNamespace("max-length-exceeded"),
        "Password must be {0} characters or less"
        );

    /// <summary>
    /// The password exactly matches a users current password. This error can only occur when a current 
    /// password is supplied with a command to authenticate the change, because the password is otherwise 
    /// unknown. Principally used by <see cref="NotCurrentPasswordNewPasswordValidator"/>.
    /// </summary>
    public static readonly ValidationErrorTemplate NotCurrentPassword = new ValidationErrorTemplate(
        AddNamespace("not-current-password"),
        "Password must not be the same as your current password."
        );

    /// <summary>
    /// The password is a sequence, or reverse sequence of characters
    /// e.g. "abcde", "54321". Principally used by <see cref="NotSequentialNewPasswordValidator"/>.
    /// </summary>
    public static readonly ValidationErrorTemplate NotSequential = new ValidationErrorTemplate(
        AddNamespace("not-sequential"),
        "Password must not be a sequence of numbers or characters."
        );

    /// <summary>
    /// The password matches an item of the users personal data, based on a case-insenstivie equality comparison.
    /// Principally used by <see cref="NotBePersonalDataNewPasswordValidator"/>.
    /// </summary>
    public static class NotPersonalData
    {
        /// <summary>
        /// The password matches the users email address, based on a case-insenstivie equality comparison.
        /// Principally used by <see cref="NotBePersonalDataNewPasswordValidator"/>.
        /// </summary>
        public static readonly ValidationErrorTemplate Email = new ValidationErrorTemplate(
            AddNotPersonalDataNamespace("not-personal-data-email"),
            "Password cannot be your email."
            );

        /// <summary>
        /// The password matches the users username, based on a case-insenstivie equality comparison.
        /// Principally used by <see cref="NotBePersonalDataNewPasswordValidator"/>.
        /// </summary>
        public static readonly ValidationErrorTemplate Username = new ValidationErrorTemplate(
            AddNotPersonalDataNamespace("username"),
            "Password cannot be your username"
            );

        private static string AddNotPersonalDataNamespace(string errorCode)
        {
            return AddNamespace("not-personal-data-" + errorCode);
        }
    }

    private static string AddNamespace(string errorCode)
    {
        return ValidationErrorCodes.AddNamespace(errorCode, "password-policy");
    }
}
