using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Utility class for parsing decimals.
    /// </summary>
    public class DecimalParser
    {
        /// <summary>
        /// Parses a string into an decimal, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Decimal value if the string could be parsed; otherwise null.</returns>
        public static decimal? ParseOrNull(string s)
        {
            decimal d = 0;
            return Decimal.TryParse(s, out d) ? d : default(decimal?);
        }

        /// <summary>
        /// Parses an object into an decimal, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="o">Object to parse</param>
        /// <returns>Decimal value if the object could be parsed; otherwise null.</returns>
        public static decimal? ParseOrNull(object o)
        {
            if (o is decimal || o is decimal?)
            {
                return o as decimal?;
            }
            return ParseOrNull(Convert.ToString(o));
        }

        /// <summary>
        /// Parses a decimal to an integer, rounding the decimal to the nearest integer value.
        /// </summary>
        /// <param name="o">Object to parse</param>
        /// <returns>Integer value.</returns>
        public static int? ParseToRoundedInt(object o)
        {
            var d = ParseOrNull(o);
            if (!d.HasValue) return null;
            return Convert.ToInt32(Math.Round(d.Value));
        }
    }
}
