using System;
using System.Collections;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for wbe directories, which are frequently requested to 
    /// work out routing
    /// </summary>
    public interface IWebDirectoryCache
    {
        /// <summary>
        /// Gets a collection of web directory routes, if the collection is already cached it 
        /// is returned, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the rewrite rules are not in the cache</param>
        WebDirectoryRoute[] GetOrAdd(Func<WebDirectoryRoute[]> getter);

        /// <summary>
        /// Clears all items in the web directory cache
        /// </summary>
        void Clear();
    }
}
