using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This enumeration represents a query for a versioned entity in a particular status.
    /// </summary>
    public enum PublishStatusQuery : short
    {
        /// <summary>
        /// Return only the published version or null if one does not exist. For this status
        /// the publish date is checked to ensure that the page is published.
        /// </summary>
        Published = 0,

        /// <summary>
        /// The latest available version, returning the latest draft if one is available
        /// and falling back to the published version.
        /// </summary>
        Latest = 1,

        /// <summary>
        /// Return only the draft version or null if one does not exist.
        /// </summary>
        Draft = 2,

        /// <summary>
        /// Returns the published version if one is available, otherwise returning a draft version.
        /// </summary>
        PreferPublished = 3,

        /// <summary>
        /// Used in combination with a version identifier to return a specific version.
        /// </summary>
        SpecificVersion = 20
    }
}
