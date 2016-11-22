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

        private const string SUMMARIES_CACHEKEY = "Summaries";
        private const string FILE_LOCATIONS_CACHEKEY = "FileLocations";
        private const string CACHEKEY = "Cofoundry.Domain.PageModuleTypeCache";

        private readonly IObjectCache _cache;

        public PageModuleTypeCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        public PageModuleTypeSummary[] GetOrAdd(Func<PageModuleTypeSummary[]> getter)
        {
            return _cache.GetOrAdd(SUMMARIES_CACHEKEY, getter);
        }

        public Task<PageModuleTypeSummary[]> GetOrAddAsync(Func<Task<PageModuleTypeSummary[]>> getter)
        {
            return _cache.GetOrAddAsync(SUMMARIES_CACHEKEY, getter);
        }

        public Dictionary<string, PageModuleTypeFileLocation> GetOrAddFileLocations(Func<Dictionary<string, PageModuleTypeFileLocation>> getter)
        {
            return _cache.GetOrAdd(FILE_LOCATIONS_CACHEKEY, getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        #endregion
    }
}
