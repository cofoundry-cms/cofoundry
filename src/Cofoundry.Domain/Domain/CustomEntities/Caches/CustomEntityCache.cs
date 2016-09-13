using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityCache : ICustomEntityCache
    {
        #region constructor

        private const string ROUTES_CACHEKEY = "Routes_";
        private const string CACHEKEY = "COF_CustomEntities";

        private readonly IObjectCache _cache;
        public CustomEntityCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods
        
        public async Task<CustomEntityRoute[]> GetOrAddAsync(string customEntityTypeCode, Func<Task<CustomEntityRoute[]>> getter)
        {
            return await _cache.GetOrAddAsync(GetEntityTypeRoutesCacheKey(customEntityTypeCode), getter);
        }

        public CustomEntityRoute[] GetOrAdd(string customEntityTypeCode, Func<CustomEntityRoute[]> getter)
        {
            return _cache.GetOrAdd(GetEntityTypeRoutesCacheKey(customEntityTypeCode), getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears all data relating to a specific custom entity
        /// </summary>
        public void Clear(string customEntityTypeCode, int customEntityId)
        {
            ClearRoutes(customEntityTypeCode);
        }

        public void ClearRoutes(string customEntityTypeCode)
        {
            _cache.Clear(GetEntityTypeRoutesCacheKey(customEntityTypeCode));
        }

        #endregion

        #region private methods

        private string GetEntityTypeRoutesCacheKey(string customEntityTypeCode)
        {
            return ROUTES_CACHEKEY + customEntityTypeCode;
        }

        #endregion
    }
}
