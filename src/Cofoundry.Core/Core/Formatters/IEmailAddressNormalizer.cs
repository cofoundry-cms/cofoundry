namespace Cofoundry.Core;

/// <summary>
/// Used to normalize email address into a consistent format.
/// The default implementation trims and lowercases the domain
/// part of the email.
/// </summary>
public interface IEmailAddressNormalizer
{
    /// <summary>
    /// Normalizes the specified email address into a consistent 
    /// format. The default implementation trims the input and lowercases the
    /// domain part of the email e.g. &quot;Test@Example.com&quot; becomes &quot;Test@example.com&quot;. 
    /// If the email is an invalid format then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="emailAddress">
    /// The email address string to format. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    NormalizedEmailAddress? NormalizeAsParts(string? emailAddress);
}
