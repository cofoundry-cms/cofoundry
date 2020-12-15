using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Caching
{
    /// <summary>
    /// Various modes that the default in-memory cache can
    /// run in.
    /// </summary>
    public enum InMemoryObjectCacheMode
    {
        /// <summary>
        /// The cache is disabled. We do not recomment this setting 
        /// unless you are debugging or troublesooting a caching 
        /// issues.
        /// </summary>
        Off,

        /// <summary>
        /// The cache lifetime is per scope i.e. all cached data is lost when
        /// the scope ends. In websites per scope is the same as per request. 
        /// This is useful if are running a multi-server configuration and don't 
        /// have access to a distributed cache.
        /// </summary>
        PerScope,

        /// <summary>
        /// The cache is persistent i.e. has singleton scope. This mode
        /// is not compatible with a multi-server configuration
        /// </summary>
        Persitent
    }
}
