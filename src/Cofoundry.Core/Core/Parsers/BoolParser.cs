using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Utility class for parsing booleans.
    /// </summary>
    public static class BoolParser
    {
        /// <summary>
        /// Parses a string into a boolean, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Boolean value if the string could be parsed; otherwise null.</returns>
        public static bool? ParseOrNull(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            bool b = false;
            if (Boolean.TryParse(s, out b))
            {
                return b;
            }

            return null;
        }

        /// <summary>
        /// Parses a string into a boolean, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Boolean value if the string could be parsed; otherwise null.</returns>
        public static bool ParseOrDefault(string s, bool def = false)
        {
            if (string.IsNullOrWhiteSpace(s)) return def;

            bool b = false;
            if (Boolean.TryParse(s, out b))
            {
                return b;
            }

            return def;
        }
    }
}
