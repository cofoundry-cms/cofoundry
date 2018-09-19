using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    public static class SlugFormatter
    {
        /// <summary>
        /// Turns the string into a url slug, removing illegal url characters and 
        /// replacing spaces with hyphens.
        /// </summary>
        /// <param name="s">string to slugify.</param>
        /// <returns>Empty string if the specified string is null or whitespace; otherwise the slugified string is returned.</returns>
        public static string ToSlug(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            string str = TextFormatter.RemoveDiacritics(s)
                .ToLower()
                .Replace("&", " and ");
            str = Regex.Replace(str, @"[/\\\.,\+=–—:_]", " ");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            str = Regex.Replace(str, @"--+", "-");

            return str.Trim('-');
        }


        /// <summary>
        /// A simple method to convert a upper or lower camel case string 
        /// to a dash delimited token, e.g. MyPropertyName to "my-property-name".
        /// </summary>
        /// <param name="s">String instance to format.</param>
        public static string CamelCaseToSlug(string s)
        {
            if (s == null) return string.Empty;

            // first separate any upper case characters
            var conveted = Regex.Replace(s, "[A-Z](?![A-Z])|[A-Z]+(?![a-z])", m => $"-{m.Value.ToLowerInvariant()}");

            // If there's any other formatting issues, the regular slugify should pick them up.
            return ToSlug(conveted);
        }
    }
}
