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
        /// delimited sentence, e.g. MyPropertyName to "My Property Name".
        /// </summary>
        /// <remarks>
        /// Used in place of Humanizer to avoid a dependency.
        /// </remarks>
        public static string PascalCaseToSentence(string s)
        {
            if (s == null) return string.Empty;

            return Regex.Replace(s, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }

        /// <summary>
        /// Converts the input words to UpperCamelCase, removing underscores, dashes and whitespace
        /// </summary>
        /// <remarks>
        /// Used in place of Humanizer to avoid a dependency. The code is adapted from 
        /// humanizer (https://github.com/Humanizr/Humanizer), but is adapted to remove 
        /// whitespace and dashes.
        /// </remarks>
        public static string Pascalize(string s)
        {
            if (s == null) return s;

            return Regex.Replace(s, @"(?:^|[_\s-])(.)", match => match.Groups[1].Value.ToUpper());
        }

        /// <summary>
        /// Converts the input words to lowerCamelCase, removing underscores, dashes and whitespace
        /// </summary>
        /// <remarks>
        /// Used in place of Humanizer to avoid a dependency. The code is adapted from 
        /// humanizer (https://github.com/Humanizr/Humanizer), but is adapted to remove 
        /// whitespace and dashes.
        /// </remarks>
        public static string Camelize(string s)
        {
            if (s == null) return s;

            var word = Pascalize(s);
            return word
                .Substring(0, 1)
                .ToLower() + word.Substring(1);
        }

        /// <summary>
        /// Substring with elipses but OK if shorter.
        /// </summary>
        public static string Limit(string s, int charCount)
        {
            if (s == null) return string.Empty;
            if (s.Length <= charCount) return s;
            else return s.Substring(0, charCount).TrimEnd();
        }

        /// <summary>
        /// Substring with elipses but OK if shorter, will take 3 characters off character count if necessary
        /// </summary>
        public static string LimitWithElipses(string s, int characterCount)
        {
            if (s == null) return string.Empty;
            if (characterCount < 5) return Limit(s, characterCount);       // Can’t do much with such a short limit
            if (s.Length <= characterCount - 3) return s;
            else return s.Substring(0, characterCount - 3) + ELIPSIS;
        }

        /// <summary>
        /// Substring with elipses but OK if shorter, will take 3 characters off character count if necessary
        /// tries to land on a space.
        /// </summary>
        public static string LimitWithElipsesOnWordBoundary(string s, int characterCount)
        {
            if (s == null) return string.Empty;
            if (characterCount < 5) return Limit(s, characterCount);       // Can’t do much with such a short limit
            if (s.Length <= characterCount - 3)
                return s;
            else
            {
                int lastspace = s.Substring(0, characterCount - 3).LastIndexOf(" ");
                if (lastspace > 0 && lastspace > characterCount - 10)
                {
                    return s.Substring(0, lastspace) + ELIPSIS;
                }
                else
                {
                    // No suitable space was found
                    return s.Substring(0, characterCount - 3) + ELIPSIS;
                }
            }
        }


        /// <summary>
        /// Trims and formats the string in title casing using the current thread culture.
        /// </summary>
        /// <param name="s">String to format</param>
        /// <returns>String formatted in title case.</returns>
        public static string ToTitleCase(string s)
        {
            if (s == null) return string.Empty;
            // Need to lower case first if string is all caps.
            if (StringHelper.IsUpperCase(s))
            {
                s = s.ToLower();
            }
            var text = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.Trim());
            return text;
        }
    }
}
