using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Utility class for parsing Int64s.
    /// </summary>
    public static class Int64Parser
    {
        /// <summary>
        /// Parses a string into an Int64, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Int64 value if the string could be parsed; otherwise null.</returns>
        public static long? ParseOrNull(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            long l = 0;
            decimal d = 0;
            if (Int64.TryParse(s, out l))
            {
                return l;
            }
            else if (decimal.TryParse(s, out d))
            {
                return Convert.ToInt64(d);
            }

            return null;
        }

        /// <summary>
        /// Parses an object into an Int64, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="o">Object to parse</param>
        /// <returns>Int64 value if the object could be parsed; otherwise null.</returns>
        public static long? ParseOrNull(object o)
        {
            if (o == null) return null;
            if (o is long || o is long?)
            {
                return o as long?;
            }
            if (o is decimal || o is decimal?)
            {
                return Convert.ToInt64(o);
            }
            return ParseOrNull(Convert.ToString(o));
        }

        /// <summary>
        /// Parses a string into an Int64, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="def">Default value to use if it cannot be parsed</param>
        /// <returns>Int64 value if the string could be parsed; otherwise the default value.</returns>
        public static long ParseOrDefault(string s, long def = 0)
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
        public static long ParseOrDefault(object o, int def = 0)
        {
            return ParseOrNull(o) ?? def;
        }
    }
}
