using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Web
{
    /// <summary>
    /// Settings to apply when setting up the static file middleware.
    /// </summary>
    public class StaticFilesSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// The default max-age to use for the cache control header, measured in
        /// seconds. The default value is 1 year. General advice here for a maximum 
        /// is 1 year.
        /// </summary>
        public int MaxAge { get; set; } = 31536000;

        /// <summary>
        /// The type of caching rule to use when adding caching headers. This defaults to
        /// StaticFileCacheMode.OnlyVersionedFiles which only sets caching headers for files 
        /// using the "v" querystring parameter convention.
        /// </summary>
        public StaticFileCacheMode CacheMode { get; set; } = StaticFileCacheMode.OnlyVersionedFiles;
    }
}