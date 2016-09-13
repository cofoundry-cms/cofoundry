using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Parser for working with collections of tags in strings, e.g. Tea, Brew, "Cup of Brown Joy".
    /// </summary>
    public static class TagParser
    {
        /// <summary>
        /// Splits a set of tags in a string into a list.
        /// Find double quoted strings, then single quoted strings
        /// (so you can have tags with spaces in) and finally splits
        /// on spaces/commas to find single word tags
        /// </summary>
        /// <param name="tags">The string of tags to split.</param>
        /// <returns>The list of separate tags</returns>
        public static IEnumerable<string> Split(string tags)
        {
            List<string> tagList = new List<string>();

            if (string.IsNullOrWhiteSpace(tags))
                return tagList;

            // Split by double quotes first
            MatchCollection doubleQuoteMatches = Regex.Matches(tags, "\"[\\S\\s]*?\"");
            foreach (Match match in doubleQuoteMatches.Cast<Match>().Reverse())
            {
                tagList.Add(match.Value.Trim(new char[] { '"' }));
                tags = tags.Remove(match.Index, match.Length);
            }

            // Then split by single quotes
            MatchCollection singleQuoteMatches = Regex.Matches(tags, "'[\\S\\s]*?'");
            foreach (Match match in singleQuoteMatches.Cast<Match>().Reverse())
            {
                tagList.Add(match.Value.Trim(new char[] { '\'' }));
                tags = tags.Remove(match.Index, match.Length);
            }

            // Then split by comma/space
            tagList.AddRange(tags.Split(new char[] { ',', ' ' })
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            return tagList
                .Select(s => s.ToLowerInvariant())
                .Distinct()
                .OrderBy(t => t);
        }
    }
}
