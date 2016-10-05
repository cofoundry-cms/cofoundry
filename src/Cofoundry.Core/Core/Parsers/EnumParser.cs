using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Utility class for parsing Enums.
    /// </summary>
    public static class EnumParser
    {
        /// <summary>
        /// Parses a string into an enum, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>TEnum value if the string could be parsed; otherwise null.</returns>
        public static TEnum? ParseOrNull<TEnum>(string s) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            TEnum e;

            if (Enum.TryParse<TEnum>(s, true, out e))
            {
                return e;
            }

            return null;
        }

        /// <summary>
        /// Parses a string into an enum, returning null if the
        /// string could not be parsed.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="defaultVaue">Default value to use if it cannot be parsed</param>
        /// <returns>TEnum value if the string could be parsed; otherwise null.</returns>
        public static TEnum ParseOrDefault<TEnum>(string s, TEnum? defaultVaue = null) where TEnum : struct
        {
            return ParseOrNull<TEnum>(s) ?? defaultVaue ?? default(TEnum);
        }
    }
}
