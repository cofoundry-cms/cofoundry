using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Utility class for parsing integers.
    /// </summary>
    public static class IntParser
    {
        /// <summary>
        /// Parses a string into an integer, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Integer value if the string could be parsed; otherwise null.</returns>
        public static int? ParseOrNull(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            int i = 0;
            decimal d = 0;
            if (Int32.TryParse(s, out i))
            {
                return i;
            }
            else if (decimal.TryParse(s, out d))
            {
                return Convert.ToInt32(d);
            }

            return null;
        }

        /// <summary>
        /// Parses an object into an integer, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="o">Object to parse</param>
        /// <returns>Integer value if the object could be parsed; otherwise null.</returns>
        public static int? ParseOrNull(object o)
        {
            if (o == null) return null;
            if (o is int || o is int?)
            {
                return o as int?;
            }
            if (o is decimal || o is decimal?)
            {
                return Convert.ToInt32(o);
            }
            return ParseOrNull(Convert.ToString(o));
        }

        /// <summary>
        /// Parses a string into an integer, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="def">Default value to use if it cannot be parsed</param>
        /// <returns>Integer value if the string could be parsed; otherwise null.</returns>
        public static int ParseOrDefault(string s, int def = 0)
        {
            return ParseOrNull(s) ?? def;
        }

        /// <summary>
        /// Parses an object into an integer, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="o">Object to parse</param>
        /// <param name="def">Default value to use if it cannot be parsed</param>
        /// <returns>Integer value if the object could be parsed; otherwise null.</returns>
        public static int ParseOrDefault(object o, int def = 0)
        {
            return ParseOrNull(o) ?? def;
        }

        /// <summary>
        /// Parses a collection of integers from a delimited string using the default delimiter (comma). 
        /// Unparsable entires are removed.
        /// </summary>
        /// <param name="list">String containing the list of integers</param>
        /// <returns>Collection of parsed integers (with unparsable entries removed)</returns>
        public static IEnumerable<int> ParseFromDelimitedString(string list)
        {
            return ParseFromDelimitedString(list, new char[] { ',' });
        }

        /// <summary>
        /// Parses a collection of integers from a delimited string. Unparsable entires are removed.
        /// </summary>
        /// <param name="str">String containing the list of integers</param>
        /// <param name="delimiter">The delimiters to pass into the string.Split operation</param>
        /// <returns>Collection of parsed integers (with unparsable entries removed)</returns>
        public static IEnumerable<int> ParseFromDelimitedString(string str, params char[] delimiter)
        {
            if (string.IsNullOrEmpty(str)) return Enumerable.Empty<int>();

            return StringHelper
                .SplitAndTrim(str, delimiter)
                .Select(s => IntParser.ParseOrNull(s))
                .FilterNotNull();
        }
    }
}
