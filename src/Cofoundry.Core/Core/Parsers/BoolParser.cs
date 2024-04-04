namespace Cofoundry.Core;

/// <summary>
/// Utility class for parsing booleans.
/// </summary>
public static class BoolParser
{
    /// <summary>
    /// Parses a string into a boolean, returning null if the
    /// string could not be parsed.
    /// </summary>
    /// <param name="s">String to parse</param>
    /// <returns>Boolean value if the string could be parsed; otherwise null.</returns>
    public static bool? ParseOrNull(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return null;
        }

        if (bool.TryParse(s, out var b))
        {
            return b;
        }

        return null;
    }

    /// <summary>
    /// Parses a string into a boolean, returning null if the
    /// string could not be parsed.
    /// </summary>
    /// <param name="s">String to parse</param>
    /// <param name="defaultValue">Optional default value.</param>
    /// <returns>Boolean value if the string could be parsed; otherwise null.</returns>
    public static bool ParseOrDefault(string? s, bool defaultValue = false)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return defaultValue;
        }

        if (bool.TryParse(s, out var b))
        {
            return b;
        }

        return defaultValue;
    }
}
