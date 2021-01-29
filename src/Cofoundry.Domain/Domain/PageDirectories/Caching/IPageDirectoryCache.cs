using System;
using System.Collections;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for page directories, which are frequently requested to 
    /// work out routing
    /// </summary>
    public interface IPageDirectoryCache
    {
        /// <summary>
        /// Gets a collection of directory routes, if the collection is already cached it 
        /// is returned, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the entities are not in the cache</param>
        ICollection<PageDirectoryRoute> GetOrAdd(Func<ICollection<PageDirectoryRoute>> getter);

        /// <summary>
        /// Clears all items in the page directory cache
        /// </summary>
        void Clear();
    }
}
