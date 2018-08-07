using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for page data, which is frequently requested to 
    /// work out routing
    /// </summary>
    public interface IPageCache
    {
        /// <summary>
        /// Gets a collection of page routes, if the collection is already cached it 
        /// is returned, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page routes are not in the cache</param>
        Task<IDictionary<int, PageRoute>> GetOrAddAsync(Func<Task<IDictionary<int, PageRoute>>> getter);

        /// <summary>
        /// Clears all items in the page cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears items in the page cache related to a specific page
        /// </summary>
        /// <param name="pageId">Id of the page to clear items for</param>
        void Clear(int pageId);

        /// <summary>
        /// Clears all page routing data from the cache
        /// </summary>
        void ClearRoutes();
    }
}
