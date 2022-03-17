namespace Cofoundry.Core;

/// <summary>
/// Validates fixed length strings of single-byte (non-unicode) characters that are the 
/// equivalent of the sql database type char. These string are often used as identifiers
/// such as for entity definitions.
/// </summary>
public class SqlCharValidator
{
    /// <summary>
    /// Validates a fixed length string to ensure it contains only single-byte (non-unicode)
    /// characters, ensuring it is the equivalent of the SQLServer database type char. Space
    /// padding is allowed but the string cannot be all spaces.
    /// </summary>
    /// <param name="length">The fixed length of the string to validate.</param>
    public static bool IsValid(string stringToValidate, int length)
    {
        return !string.IsNullOrWhiteSpace(stringToValidate)
                        && stringToValidate.Length == length
                        && !stringToValidate.Any(c => c > 255);
    }
}
