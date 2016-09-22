using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// A range of helper methods for working with strings or collections of strings
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Returns the first non empty string in an array
        /// </summary>
        public static string FirstNonEmpty(params string[] strings)
        {
            return strings.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? string.Empty;
        }

        /// <summary>
        /// Trims whitespace from a string, returning null values as String.Empty
        /// </summary>
        public static string NullAsEmptyAndTrim(string s)
        {
            if (s == null) return string.Empty;

            return s.Trim();
        }

        /// <summary>
        /// Trims whitespace from a string if not null
        /// </summary>
        public static string TrimOrNull(string s)
        {
            if (s == null) return s;

            return s.Trim();
        }

        /// <summary>
        /// Returns a string as null if it is empty or whitespace
        /// </summary>
        public static string EmptyAsNull(string s)
        {
            if (s == null || !string.IsNullOrWhiteSpace(s)) return s;
            return null;
        }

        /// <summary>
        /// Returns true if any of the string parameters are null, empty or whitespace
        /// </summary>
        public static bool IsNullOrWhiteSpace(params string[] strings)
        {
            if (strings == null) return true;
            return strings.Any(s => string.IsNullOrWhiteSpace(s));
        }

        /// <summary>
        /// Concatenates the members of a collection of strings, removing empty entries and using the specified separator between each
        ///  member.
        /// </summary>
        /// <param name="separator">The string to use as a separator.</param>
        /// <param name="values">A collection that contains the strings to concatenate.</param>
        /// <returns>
        /// A string that consists of the non-empty members of values delimited by the separator
        /// string. If values has no non-empty members, the method returns System.String.Empty.
        /// </returns>
        public static string JoinNotEmpty(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        /// <summary>
        /// Splits a string, removing empty entires and trimming each entry.
        /// </summary>
        /// <param name="source">String to split</param>
        /// <returns>Collection of string results.</returns>
        public static IEnumerable<string> SplitAndTrim(string source, params char[] separator)
        {
            if (source == null) return Enumerable.Empty<string>();

            return source
                .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s != string.Empty);
        }

        public static bool IsUpperCase(string s)
        {
            if (s == null) return false;
            return !s.Any(c => !Char.IsUpper(c));
        }
    }
}
