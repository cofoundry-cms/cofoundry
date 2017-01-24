using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for wbe directories, which are frequently requested to 
    /// work out routing
    /// </summary>
    public class WebDirectoryCache : IWebDirectoryCache
    {
        #region constructor

        private const string WEBDIRECTORYROUTES_CACHEKEY = "PageRoutes";
        private const string CACHEKEY = "COF_WebDirectories";

        private readonly IObjectCache _cache;
        private readonly IPageCache _pageCache;

        public WebDirectoryCache(
            IObjectCacheFactory cacheFactory,
            IPageCache pageCache
            )
        {
            _cache = cacheFactory.Get(CACHEKEY);
            _pageCache = pageCache;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets a collection of web directory routes, if the collection is already cached it 
        /// is returned, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the rewrite rules are not in the cache</param>
        public WebDirectoryRoute[] GetOrAdd(Func<WebDirectoryRoute[]> getter)
        {
            return _cache.GetOrAdd(WEBDIRECTORYROUTES_CACHEKEY, getter);
        }

        /// <summary>
        /// Clears all items in the web directory cache. This also clears out
        /// the pages cache because page routes are dependent on web directories.
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
            _pageCache.Clear();
        }

        #endregion
    }
}
