using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to indicate the type of validation to perform
    /// validating a file extension or mime type against
    /// a list of values.
    /// </summary>
    public enum AssetFileTypeValidation
    {
        /// <summary>
        /// Treat the list of values as a blacklist, ensuring
        /// the value being validated does not appear in the
        /// list.
        /// </summary>
        UseBlacklist,

        /// <summary>
        /// Treat the list of values as a whitelist, ensuring
        /// the value being validated appears in the list.
        /// </summary>
        UseWhitelist,

        /// <summary>
        /// Disable validation completely.
        /// </summary>
        Disabled
    }
}
