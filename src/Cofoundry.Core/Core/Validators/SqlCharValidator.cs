using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Validates fixed length non-unicode strings that are the equivalent of the
    /// sql database type char. These string are often used as identifiers
    /// such as for entity definitions.
    /// </summary>
    public class SqlCharValidator
    {
        /// <summary>
        /// Validates a fixed length non-unicode string to ensure it is the equivalent 
        /// of the sql database type char. Psace padding is allowed but the string cannot
        /// be all spaces.
        /// </summary>
        /// <param name="length">The fixed length of the string to validate.</param>
        public static bool IsValid(string stringToValidate, int length)
        {
            return !string.IsNullOrWhiteSpace(stringToValidate)
                            && stringToValidate.Length == length
                            && !stringToValidate.Any(c => c > 255);
        }
    }
}
