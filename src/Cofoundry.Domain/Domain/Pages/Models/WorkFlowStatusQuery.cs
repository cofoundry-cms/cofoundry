using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This enumeration represents a query for a page in a particular status
    /// </summary>
    public enum WorkFlowStatusQuery
    {
        /// <summary>
        /// The latest available version, returning the latest draft if one is available
        /// and falling back to the published version.
        /// </summary>
        Latest = 0,

        /// <summary>
        /// Return only the draft version or null if one does not exist
        /// </summary>
        Draft = 1,

        /// <summary>
        /// Return only the published version or null if one does not exist
        /// </summary>
        Published = 4,

        /// <summary>
        /// Used in combination with a PageVersionId to return a specific version
        /// </summary>
        SpecificVersion = 20,

        /// <summary>
        /// Returns the published version if one is available, otherwise returning a draft version.
        /// </summary>
        PreferPublished = 21
    }
}
