namespace Cofoundry.Core;

public static class IEmailAddressNormalizerExtensions
{
    /// <summary>
    /// Normalizes the specified email address into a consistent 
    /// format. The default implementation trims the input and lowercases the
    /// domain part of the email. If the email is an invalid format then 
    /// <see langword="null"/> is returned.
    /// </summary>
    /// <param name="normalizer">
    /// <see cref="IEmailAddressNormalizer"/> instance to extend.
    /// </param>
    /// <param name="emailAddress">
    /// The email address string to format. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    public static string? Normalize(this IEmailAddressNormalizer normalizer, string? emailAddress)
    {
        ArgumentNullException.ThrowIfNull(normalizer);

        var parts = normalizer.NormalizeAsParts(emailAddress);
        return parts?.ToEmailAddress();
    }
}
