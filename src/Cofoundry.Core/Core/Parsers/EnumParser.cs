using System;

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
        /// <param name="value"><see cref="String"/> to parse.</param>
        /// <returns><typeparamref name="TEnum"/> value if <paramref name="value"/> could be parsed; otherwise <see langword="null"/>.</returns>
        public static TEnum? ParseOrNull<TEnum>(string value) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            TEnum e;

            if (Enum.TryParse<TEnum>(value, true, out e))
            {
                return e;
            }

            return null;
        }

        /// <summary>
        /// Converts the <see cref="int"/> value to <typeparamref name="TEnum"/>, validating that <paramref name="value"/>
        /// exists in the range of declared values of <typeparamref name="TEnum"/>. If <paramref name="value"/>
        /// is not a valid <typeparamref name="TEnum"/> value then <see langword="null"/> is returned.
        /// is thrown.
        /// </summary>
        /// <typeparam name="TEnum">Enum type to convert to.</typeparam>
        /// <param name="value">The value to convert from.</param>
        /// <returns><typeparamref name="TEnum"/> value if <paramref name="value"/> could be parsed; otherwise <see langword="null"/>.</returns>
        public static TEnum? ParseOrNull<TEnum>(int value) where TEnum : struct
        {
            return ParseNumericOrNull<TEnum>(value);
        }

        /// <summary>
        /// Converts the <see cref="short"/> value to <typeparamref name="TEnum"/>, validating that <paramref name="value"/>
        /// exists in the range of declared values of <typeparamref name="TEnum"/>. If <paramref name="value"/>
        /// is not a valid <typeparamref name="TEnum"/> value then <see langword="null"/> is returned.
        /// is thrown.
        /// </summary>
        /// <typeparam name="TEnum">Enum type to convert to.</typeparam>
        /// <param name="value">The value to convert from.</param>
        /// <returns><typeparamref name="TEnum"/> value if <paramref name="value"/> could be parsed; otherwise <see langword="null"/>.</returns>
        public static TEnum? ParseOrNull<TEnum>(short value) where TEnum : struct
        {
            return ParseNumericOrNull<TEnum>(value);
        }

        private static TEnum? ParseNumericOrNull<TEnum>(object value)
            where TEnum : struct
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                return null;
            }

            return (TEnum)value;
        }

        /// <summary>
        /// Parses a string into an enum, returning the specified <paramref name="defaultValue"/> 
        /// if it could not be parsed.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <param name="defaultValue">Default value to use if it cannot be parsed.</param>
        /// <returns>
        /// <typeparamref name="TEnum"/> value if the string could be parsed; otherwise <paramref name="defaultValue"/>. 
        /// If <paramref name="defaultValue"/> if null then the default value for <typeparamref name="TEnum"/> is 
        /// returned instead, however the default value for an enum is 0, so this may not be a valid <typeparamref name="TEnum"/> 
        /// value.
        /// </returns>
        public static TEnum ParseOrDefault<TEnum>(string s, TEnum? defaultValue = null) where TEnum : struct
        {
            return ParseOrNull<TEnum>(s) ?? defaultValue ?? default(TEnum);
        }

        /// <summary>
        /// Converts the <see cref="int"/> value to <typeparamref name="TEnum"/>, validating that <paramref name="value"/>
        /// exists in the range of declared values of <typeparamref name="TEnum"/>. If <paramref name="value"/>
        /// is not a valid <typeparamref name="TEnum"/> value then an <see cref="ArgumentException"/>
        /// is thrown.
        /// </summary>
        /// <typeparam name="TEnum">Enum type to convert to.</typeparam>
        /// <param name="value">The value to convert from.</param>
        /// <returns>Converted <typeparamref name="TEnum"/> value.</returns>
        public static TEnum ParseOrThrow<TEnum>(int value) where TEnum : struct
        {
            return ParseNumericOrThrow<TEnum>(value);
        }

        /// <summary>
        /// Converts the <see cref="short"/> value to <typeparamref name="TEnum"/>, validating that <paramref name="value"/>
        /// exists in the range of declared values of <typeparamref name="TEnum"/>. If <paramref name="value"/>
        /// is not a valid <typeparamref name="TEnum"/> value then an <see cref="ArgumentException"/>
        /// is thrown.
        /// </summary>
        /// <typeparam name="TEnum">Enum type to convert to.</typeparam>
        /// <param name="value">The value to convert from.</param>
        /// <returns>Converted <typeparamref name="TEnum"/> value.</returns>
        public static TEnum ParseOrThrow<TEnum>(short value) where TEnum : struct
        {
            return ParseNumericOrThrow<TEnum>(value);
        }

        private static TEnum ParseNumericOrThrow<TEnum>(object value)
            where TEnum : struct
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new ArgumentException($"{value} is not a valid {typeof(TEnum).Name} value.");
            }

            return (TEnum)value;
        }
    }
}
