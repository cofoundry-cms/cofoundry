using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Cache for page data, which is frequently requested to 
    /// work out routing
    /// </summary>
    public class PageCache : IPageCache
    {
        #region constructor
        
        private const string PAGEROUTES_CACHEKEY = "PageRoutes";
        private const string CACHEKEY = "COF_Pages";

        private readonly IObjectCache _cache;
        public PageCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets a collection of page routes, if the collection is already cached it 
        /// is returned, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page routes are not in the cache</param>
        public Task<IDictionary<int, PageRoute>> GetOrAddAsync(Func<Task<IDictionary<int, PageRoute>>> getter)
        {
            return _cache.GetOrAddAsync(PAGEROUTES_CACHEKEY, getter);
        }

        /// <summary>
        /// Clears all items in the page cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears items in the page cache related to a specific page
        /// </summary>
        /// <param name="pageId">Id of the page to clear items for</param>
        public void Clear(int pageId)
        {
           ClearRoutes();
        }

        /// <summary>
        /// Clears all page routing data from the cache
        /// </summary>
        public void ClearRoutes()
        {
            _cache.Clear(PAGEROUTES_CACHEKEY);
        }

        #endregion
    }
}
