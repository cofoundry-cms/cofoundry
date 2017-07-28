using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Indicates the type of rule to apply when adding caching 
    /// headers to static files.
    /// </summary>
    public enum StaticFileCacheMode
    {
        /// <summary>
        /// No cache headers are set.
        /// </summary>
        None,

        /// <summary>
        /// Cache headers are set for files using the "v" querystring parameter convention.
        /// </summary>
        OnlyVersionedFiles,

        /// <summary>
        /// Add cache headers for all files.
        /// </summary>
        All
    }
}
