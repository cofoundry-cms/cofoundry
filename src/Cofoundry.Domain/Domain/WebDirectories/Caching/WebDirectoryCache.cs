using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
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

        public WebDirectoryRoute[] GetOrAdd(Func<WebDirectoryRoute[]> getter)
        {
            return _cache.GetOrAdd(WEBDIRECTORYROUTES_CACHEKEY, getter);
        }

        public void Clear()
        {
            _cache.Clear();
            _pageCache.Clear();
        }

        #endregion
    }
}
