using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used by <see cref="EmailAddressValidator"/> to validate the formatting
    /// of an email address.
    /// </summary>
    public static class EmailAddressCharacterValidator
    {
        /// <summary>
        /// Validates the specified <paramref name="emailAddress"/>, returning
        /// any charcters that are not valid according to the rules set out
        /// in the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="emailAddress">The email to validate.</param>
        /// <param name="options">The configuraton options to use during validation.</param>
        /// <returns>Enumerable collection of invalid characters.</returns>
        public static IEnumerable<char> GetInvalidCharacters(string emailAddress, EmailAddressOptions options)
        {
            if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.AllowAnyCharacter) return Array.Empty<char>();

            // Always allow @ because an email is invalid without it
            var badCharacters = emailAddress
                .Distinct()
                .Where(c => c != '@');

            if (options.AllowAnyDigit)
            {
                badCharacters = badCharacters.Where(c => !Char.IsDigit(c));
            }

            if (options.AllowAnyLetter)
            {
                badCharacters = badCharacters.Where(c => !Char.IsLetter(c));
            }

            if (!string.IsNullOrEmpty(options.AdditionalAllowedCharacters))
            {
                badCharacters = badCharacters.Where(c => !options.AdditionalAllowedCharacters.Contains(c));
            }

            return badCharacters;
        }
    }
}
