using System.Globalization;

namespace Cofoundry.Core;

/// <summary>
/// Utility class for parsing decimals.
/// </summary>
public class DecimalParser
{
    /// <summary>
    /// Parses a string into an decimal, returning <see langword="null"/>
    /// if the string could not be parsed.
    /// </summary>
    /// <param name="s">String to parse</param>
    /// <returns>Decimal value if the string could be parsed; otherwise <see langword="null"/>.</returns>
    public static decimal? ParseOrNull(string? s)
    {
        return decimal.TryParse(s, out var d) ? d : default(decimal?);
    }

    /// <summary>
    /// Parses an <see langword="object"/> into a <see langword="decimal"/>, 
    /// returning <see langword="null"/> if the string could not be parsed.
    /// </summary>
    /// <param name="o">Object to parse</param>
    /// <returns>Decimal value if the object could be parsed; otherwise <see langword="null"/>.</returns>
    public static decimal? ParseOrNull(object? o)
    {
        if (o is decimal || o is decimal?)
        {
            return o as decimal?;
        }
        return ParseOrNull(Convert.ToString(o, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Parses a <see langword="decimal"/> to an <see langword="int"/>, rounding to the 
    /// nearest integer value.
    /// </summary>
    /// <param name="o">Object to parse</param>
    /// <returns>Integer value.</returns>
    public static int? ParseToRoundedInt(object? o)
    {
        var d = ParseOrNull(o);
        if (!d.HasValue)
        {
            return null;
        }

        return Convert.ToInt32(Math.Round(d.Value));
    }
}
