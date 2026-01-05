namespace Cofoundry.Core;

/// <summary>
/// Extension methods for <see cref="IEmailAddressNormalizer"/>.
/// </summary>
public static class IEmailAddressNormalizerExtensions
{
    extension(IEmailAddressNormalizer normalizer)
    {
        /// <summary>
        /// Normalizes the specified email address into a consistent 
        /// format. The default implementation trims the input and lowercases the
        /// domain part of the email. If the email is an invalid format then 
        /// <see langword="null"/> is returned.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address string to format. If the value is <see langword="null"/> 
        /// then <see langword="null"/> is returned.
        /// </param>
        public string? Normalize(string? emailAddress)
        {
            ArgumentNullException.ThrowIfNull(normalizer);

            var parts = normalizer.NormalizeAsParts(emailAddress);
            return parts?.ToEmailAddress();
        }
    }
}
