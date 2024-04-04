using System.Globalization;
using System.Text.RegularExpressions;

namespace Cofoundry.Core;

/// <summary>
/// A range of helper methods for formatting text string
/// </summary>
public static partial class TextFormatter
{
    private const string ELIPSIS = "…";

    /// <summary>
    /// A simple method to convert a pascal case string to a space
    /// delimited sentence, e.g. MyPropertyName to "My property name".
    /// </summary>
    /// <param name="s">String instance to format.</param>
    public static string PascalCaseToSentence(string? s)
    {
        if (s == null)
        {
            return string.Empty;
        }

        var conveted = PascalCaseToSentenceRegex().Replace(s, m =>
        {
            var transformed = m.Value.Length > 1 ? m.Value : m.Value.ToLowerInvariant();
            return " " + transformed;
        });

        return conveted;
    }

    /// <summary>
    /// Converts the input words to UpperCamelCase, removing underscores, dashes and whitespace
    /// </summary>
    /// <remarks>
    /// Used in place of Humanizer to avoid a dependency. The code is adapted from 
    /// humanizer (https://github.com/Humanizr/Humanizer), but is adapted to remove 
    /// whitespace and dashes.
    /// </remarks>
    /// <param name="s">String instance to format.</param>
    public static string Pascalize(string? s)
    {
        if (s == null)
        {
            return string.Empty;
        }

        return PascalizeRegex().Replace(s.Trim(), match => match.Groups[1].Value.ToUpper());
    }

    /// <summary>
    /// Converts the input words to lowerCamelCase, removing underscores, dashes and whitespace
    /// </summary>
    /// <remarks>
    /// Used in place of Humanizer to avoid a dependency. The code is adapted from 
    /// humanizer (https://github.com/Humanizr/Humanizer), but is adapted to remove 
    /// whitespace and dashes.
    /// </remarks>
    /// <param name="s">String instance to format.</param>
    public static string Camelize(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return string.Empty;
        }

        var word = Pascalize(s);

        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        return string.Concat(
            word.Substring(0, 1).ToLower(),
            word.AsSpan(1)
            );
    }

    /// <summary>
    /// Truncates <paramref name="text"/> to the maximum allowed characters.
    /// </summary>
    /// <param name="text">Text to limit or truncate.</param>
    /// <param name="maxLength">The maximum number of characters to allow before truncating <paramref name="text"/>.</param>
    public static string Limit(string? text, int maxLength)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if (text.Length <= maxLength)
        {
            return text;
        }
        else
        {
            return text.Substring(0, maxLength).TrimEnd();
        }
    }

    /// <summary>
    /// Truncates <paramref name="text"/> to the maximum allowed characters, using
    /// an elipsis to mark the end of the text.
    /// </summary>
    /// <param name="text">Text to limit or truncate.</param>
    /// <param name="maxLength">The maximum number of characters to return in the truncated text, including the elipsis.</param>
    public static string LimitWithElipses(string? text, int maxLength)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if (maxLength <= ELIPSIS.Length)
        {
            // Can’t do much with such a short limit
            return Limit(text, maxLength);
        }

        if (text.Length <= maxLength)
        {
            return text;
        }
        else
        {
            return text.Substring(0, maxLength - ELIPSIS.Length).TrimEnd() + ELIPSIS;
        }
    }

    /// <summary>
    /// Truncates <paramref name="text"/> to the maximum allowed characters, using
    /// an elipsis to mark the end of the text. The algorithm avoids truncating a word
    /// unless an appropriate split point cannot be found.
    /// </summary>
    /// <param name="text">Text to limit or truncate.</param>
    /// <param name="maxLength">The maximum number of characters to return in the truncated text, including the elipsis.</param>
    public static string LimitWithElipsesOnWordBoundary(string? text, int maxLength)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if (maxLength <= ELIPSIS.Length)
        {
            // Can’t do much with such a short limit
            return Limit(text, maxLength);
        }

        if (text.Length <= maxLength)
        {
            return text;
        }
        else
        {
            var lastspace = text.Substring(0, maxLength - (1 - ELIPSIS.Length)).LastIndexOf(' ');
            if (lastspace > 0 && lastspace > maxLength - 10)
            {
                return string.Concat(text.AsSpan(0, lastspace), ELIPSIS);
            }
            else
            {
                // No suitable space was found
                return string.Concat(text.AsSpan(0, maxLength - ELIPSIS.Length), ELIPSIS);
            }
        }
    }

    /// <summary>
    /// Converts the first letter in the string to upper case. If <paramref name="text"/>
    /// is <see langword="null"/> then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="text">Text to format.</param>
    public static string FirstLetterToUpperCase(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var a = text.ToCharArray();
        a[0] = char.ToUpper(a[0]);

        return new string(a);
    }

    #region RemoveDiacritics

    /// <summary>
    /// Removes diacritics (accents) from a string.
    /// </summary>
    /// <remarks>
    /// Adapted from https://stackoverflow.com/a/34272324/716689
    /// </remarks>
    /// <param name="s">The string from which to remove diacratics.</param>
    public static string RemoveDiacritics(string? s)
    {
        if (s == null)
        {
            return string.Empty;
        }

        var characters = s.SelectMany(TranslateCharacter);
        var result = new string(characters.ToArray());

        return result;
    }

    private static readonly Lazy<Dictionary<char, string>> _charcterLookup = new(CreateCharacterCache);

    private static Dictionary<char, string> CreateCharacterCache()
    {
        #region lookup source

        var sourceMappings = new Dictionary<string, string>
        {
            { "äæǽ", "ae" },
            { "öœ", "oe" },
            { "ü", "ue" },
            { "Ä", "Ae" },
            { "Ü", "Ue" },
            { "Ö", "Oe" },
            { "ÀÁÂÃÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "A" },
            { "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "a" },
            { "Б", "B" },
            { "б", "b" },
            { "ÇĆĈĊČ", "C" },
            { "çćĉċč", "c" },
            { "Д", "D" },
            { "д", "d" },
            { "ÐĎĐΔ", "Dj" },
            { "ðďđδ", "dj" },
            { "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
            { "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
            { "Ф", "F" },
            { "ф", "f" },
            { "ĜĞĠĢΓГҐ", "G" },
            { "ĝğġģγгґ", "g" },
            { "ĤĦ", "H" },
            { "ĥħ", "h" },
            { "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
            { "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
            { "Ĵ", "J" },
            { "ĵ", "j" },
            { "ĶΚК", "K" },
            { "ķκк", "k" },
            { "ĹĻĽĿŁΛЛ", "L" },
            { "ĺļľŀłλл", "l" },
            { "М", "M" },
            { "м", "m" },
            { "ÑŃŅŇΝН", "N" },
            { "ñńņňŉνн", "n" },
            { "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО", "O" },
            { "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "o" },
            { "П", "P" },
            { "п", "p" },
            { "ŔŖŘΡР", "R" },
            { "ŕŗřρр", "r" },
            { "ŚŜŞȘŠΣС", "S" },
            { "śŝşșšſσςс", "s" },
            { "ȚŢŤŦτТ", "T" },
            { "țţťŧт", "t" },
            { "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛỦỤỪỨỮỬỰУ", "U" },
            { "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "u" },
            { "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
            { "ýÿŷỳỹỷỵй", "y" },
            { "В", "V" },
            { "в", "v" },
            { "Ŵ", "W" },
            { "ŵ", "w" },
            { "ŹŻŽΖЗ", "Z" },
            { "źżžζз", "z" },
            { "ÆǼ", "AE" },
            { "ß", "ss" },
            { "Ĳ", "IJ" },
            { "ĳ", "ij" },
            { "Œ", "OE" },
            { "ƒ", "f" },
            { "ξ", "ks" },
            { "π", "p" },
            { "β", "v" },
            { "μ", "m" },
            { "ψ", "ps" },
            { "Ё", "Yo" },
            { "ё", "yo" },
            { "Є", "Ye" },
            { "є", "ye" },
            { "Ї", "Yi" },
            { "Ж", "Zh" },
            { "ж", "zh" },
            { "Х", "Kh" },
            { "х", "kh" },
            { "Ц", "Ts" },
            { "ц", "ts" },
            { "Ч", "Ch" },
            { "ч", "ch" },
            { "Ш", "Sh" },
            { "ш", "sh" },
            { "Щ", "Shch" },
            { "щ", "shch" },
            { "ЪъЬь", "" },
            { "Ю", "Yu" },
            { "ю", "yu" },
            { "Я", "Ya" },
            { "я", "ya" },
        };

        #endregion

        var characterLookup = new Dictionary<char, string>();

        foreach (var sourceMapping in sourceMappings)
        {
            foreach (var originalChar in sourceMapping.Key)
            {
                characterLookup.Add(originalChar, sourceMapping.Value);
            }
        }

        return characterLookup;
    }

    private static IEnumerable<char> TranslateCharacter(char characterToTranslate)
    {
        if (_charcterLookup.Value.TryGetValue(characterToTranslate, out var value))
        {
            foreach (var character in value)
            {
                yield return character;
            }
        }
        else if (CharUnicodeInfo.GetUnicodeCategory(characterToTranslate) != UnicodeCategory.NonSpacingMark)
        {
            yield return characterToTranslate;
        }
    }

    #endregion

    [GeneratedRegex("(?<=.)[A-Z](?![A-Z])|[A-Z]+(?![a-z])")]
    private static partial Regex PascalCaseToSentenceRegex();

    [GeneratedRegex(@"(?:^|[_\s-])(.)")]
    private static partial Regex PascalizeRegex();
}
