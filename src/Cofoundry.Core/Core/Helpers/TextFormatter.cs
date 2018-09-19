using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// A range of helper methods for formatting text string
    /// </summary>
    public static class TextFormatter
    {
        #region constants

        private const string ELIPSIS = "…";

        #endregion

        /// <summary>
        /// A simple method to convert a pascal case string to a space
        /// delimited sentence, e.g. MyPropertyName to "My property name".
        /// </summary>
        /// <param name="s">String instance to format.</param>
        public static string PascalCaseToSentence(string s)
        {
            if (s == null) return string.Empty;

            var conveted = Regex.Replace(s, "(?<=.)[A-Z](?![A-Z])|[A-Z]+(?![a-z])", m => 
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
        public static string Pascalize(string s)
        {
            if (s == null) return string.Empty;

            return Regex.Replace(s.Trim(), @"(?:^|[_\s-])(.)", match => match.Groups[1].Value.ToUpper());
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
        public static string Camelize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            var word = Pascalize(s);

            if (string.IsNullOrEmpty(word)) return string.Empty;

            return word
                .Substring(0, 1)
                .ToLower() + word.Substring(1);
        }

        /// <summary>
        /// Substring with elipses but OK if shorter.
        /// </summary>
        /// <param name="s">String instance to format.</param>
        public static string Limit(string s, int charCount)
        {
            if (s == null) return string.Empty;
            if (s.Length <= charCount) return s;
            else return s.Substring(0, charCount).TrimEnd();
        }

        /// <summary>
        /// Substring with elipses but OK if shorter, will take 3 characters off character count if necessary
        /// </summary>
        /// <param name="s">String instance to format.</param>
        public static string LimitWithElipses(string s, int characterCount)
        {
            if (s == null) return string.Empty;
            if (characterCount < 3) return Limit(s, characterCount);       // Can’t do much with such a short limit
            if (s.Length <= characterCount) return s;
            else return s.Substring(0, characterCount - ELIPSIS.Length).TrimEnd() + ELIPSIS;
        }

        /// <summary>
        /// Substring with elipses but OK if shorter, will take 3 characters off character count if necessary
        /// tries to land on a space.
        /// </summary>
        /// <param name="s">String instance to format.</param>
        public static string LimitWithElipsesOnWordBoundary(string s, int characterCount)
        {
            if (s == null) return string.Empty;
            if (characterCount < 3) return Limit(s, characterCount);       // Can’t do much with such a short limit
            if (s.Length <= characterCount)
            {
                return s;
            }
            else
            {
                int lastspace = s.Substring(0, characterCount - (1 - ELIPSIS.Length)).LastIndexOf(" ");
                if (lastspace > 0 && lastspace > characterCount - 10)
                {
                    return s.Substring(0, lastspace) + ELIPSIS;
                }
                else
                {
                    // No suitable space was found
                    return s.Substring(0, characterCount - ELIPSIS.Length) + ELIPSIS;
                }
            }
        }

        /// <summary>
        /// Converts the first letter in the string to upper case. If the string is null
        /// then null is returned.
        /// </summary>
        /// <param name="s">String instance to format.</param>
        public static string FirstLetterToUpperCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            char[] a = s.ToCharArray();
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
        public static string RemoveDiacritics(string s)
        {
            if (s == null) return string.Empty;

            var characters = s.SelectMany(TranslateCharacter);
            string result = new string(characters.ToArray());

            return result;
        }

        private static Lazy<Dictionary<char, string>> _charcterLookup = new Lazy<Dictionary<char, string>>(CreateCharacterCache);

        private static Dictionary<char, string> CreateCharacterCache()
        {
            #region lookup source

            Dictionary<string, string> sourceMappings = new Dictionary<string, string>
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
                foreach (var originalChar in sourceMapping.Key)
                {
                    characterLookup.Add(originalChar, sourceMapping.Value);
                }

            return characterLookup;
        }

        private static IEnumerable<char> TranslateCharacter(char characterToTranslate)
        {
            if (_charcterLookup.Value.ContainsKey(characterToTranslate))
            {
                foreach (var character in _charcterLookup.Value[characterToTranslate])
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
    }
}
