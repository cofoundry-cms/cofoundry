namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used by <see cref="Extendable.UsernameValidator"/> to validate the formatting
/// of a username.
/// </summary>
public static class UsernameCharacterValidator
{
    /// <summary>
    /// Validates the specified <paramref name="username"/>, returning
    /// any charcters that are not valid according to the rules set out
    /// in the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <param name="options">The configuraton options to use during validation.</param>
    /// <returns>Enumerable collection of invalid characters.</returns>
    public static IEnumerable<char> GetInvalidCharacters(string username, UsernameOptions options)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(options);

        if (options.AllowAnyCharacter)
        {
            return Array.Empty<char>();
        }

        var badCharacters = username.Distinct();

        if (options.AllowAnyDigit)
        {
            badCharacters = badCharacters.Where(c => !char.IsDigit(c));
        }

        if (options.AllowAnyLetter)
        {
            badCharacters = badCharacters.Where(c => !char.IsLetter(c));
        }

        if (!string.IsNullOrEmpty(options.AdditionalAllowedCharacters))
        {
            badCharacters = badCharacters.Where(c => !options.AdditionalAllowedCharacters.Contains(c));
        }

        return badCharacters;
    }
}
