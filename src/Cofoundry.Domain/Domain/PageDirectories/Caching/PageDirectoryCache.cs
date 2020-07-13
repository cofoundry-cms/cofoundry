using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for web directories, which are frequently requested to 
    /// work out routing.
    /// </summary>
    public class PageDirectoryCache : IPageDirectoryCache
    {
        private const string PAGEDIRECTORYROUTES_CACHEKEY = "PageRoutes";
        private const string CACHEKEY = "COF_PageDirectories";

        private readonly IObjectCache _cache;
        private readonly IPageCache _pageCache;

        public PageDirectoryCache(
            IObjectCacheFactory cacheFactory,
            IPageCache pageCache
            )
        {
            _cache = cacheFactory.Get(CACHEKEY);
            _pageCache = pageCache;
        }

        /// <summary>
        /// Gets a collection of page directory routes, if the collection is already cached it 
        /// is returned, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the rewrite rules are not in the cache</param>
        public ICollection<PageDirectoryRoute> GetOrAdd(Func<ICollection<PageDirectoryRoute>> getter)
        {
            return _cache.GetOrAdd(PAGEDIRECTORYROUTES_CACHEKEY, getter);
        }

        /// <summary>
        /// Clears all items in the page directory cache. This also clears out
        /// the pages cache because page routes are dependent on page directories.
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
            _pageCache.Clear();
        }
    }
}
