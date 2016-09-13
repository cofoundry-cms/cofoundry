using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageCache : IPageCache
    {
        #region constructor

        private const string MODULETYPESUMMARY_CACHEKEY = "ModuleTypeSummaries";
        private const string PAGEROUTES_CACHEKEY = "PageRoutes";
        private const string CACHEKEY = "COF_Pages";

        private readonly IObjectCache _cache;
        public PageCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        public PageRoute[] GetOrAdd(Func<PageRoute[]> getter)
        {
            return _cache.GetOrAdd(PAGEROUTES_CACHEKEY, getter);
        }

        public async Task<PageRoute[]> GetOrAddAsync(Func<Task<PageRoute[]>> getter)
        {
            return await _cache.GetOrAddAsync(PAGEROUTES_CACHEKEY, getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears all data relating to a specific page
        /// </summary>
        public void Clear(int pageId)
        {
           ClearRoutes();
        }

        public void ClearRoutes()
        {
            _cache.Clear(PAGEROUTES_CACHEKEY);
        }

        #endregion
    }
}
