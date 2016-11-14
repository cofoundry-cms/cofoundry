using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageModuleTypeCache : IPageModuleTypeCache
    {
        #region constructor

        private const string MODULETYPESUMMARY_CACHEKEY = "ModuleTypeSummaries";
        private const string CACHEKEY = "COF_PageModules";

        private readonly IObjectCache _cache;

        public PageModuleTypeCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        public PageModuleTypeSummary[] GetOrAdd(Func<PageModuleTypeSummary[]> getter)
        {
            return _cache.GetOrAdd(MODULETYPESUMMARY_CACHEKEY, getter);
        }

        public async Task<PageModuleTypeSummary[]> GetOrAddAsync(Func<Task<PageModuleTypeSummary[]>> getter)
        {
            return await _cache.GetOrAddAsync(MODULETYPESUMMARY_CACHEKEY, getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        #endregion
    }
}
