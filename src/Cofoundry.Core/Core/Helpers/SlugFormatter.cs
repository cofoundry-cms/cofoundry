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
                .Replace(" & ", " and ")
                .Replace("/", " ")
                .Replace("\\", " ");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            str = Regex.Replace(str, @"--+", "-");
            return str;
        }
    }
}
