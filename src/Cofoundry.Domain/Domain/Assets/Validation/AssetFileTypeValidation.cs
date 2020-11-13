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
        /// Treat the list of values as a blocklist, ensuring
        /// the value being validated does not appear in the
        /// list.
        /// </summary>
        UseBlockList,

        /// <summary>
        /// Treat the list of values as an allowlist, ensuring
        /// the value being validated appears in the list.
        /// </summary>
        UseAllowList,

        /// <summary>
        /// Disable validation completely.
        /// </summary>
        Disabled
    }
}
