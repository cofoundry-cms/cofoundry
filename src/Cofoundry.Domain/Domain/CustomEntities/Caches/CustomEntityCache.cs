using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Cache for custom entity data, particularly CustomEntityRoute objects
    /// which are frequently requested to work out routing
    /// </summary>
    public class CustomEntityCache : ICustomEntityCache
    {
        private const string ROUTES_CACHEKEY = "Routes_";
        private const string CACHEKEY = "COF_CustomEntities";

        private readonly IObjectCache _cache;
        public CustomEntityCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        /// <summary>
        /// Gets a collection of custom entity routes for the specified
        /// custom entity type. If the colelction is already cached it 
        /// is returned , otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="customEntityTypeCode">Definition code of the custom entity type to return routes for</param>
        /// <param name="getter">Function to invoke if the custom entities are not in the cache</param>
        public async Task<ICollection<CustomEntityRoute>> GetOrAddAsync(string customEntityTypeCode, Func<Task<ICollection<CustomEntityRoute>>> getter)
        {
            return await _cache.GetOrAddAsync(GetEntityTypeRoutesCacheKey(customEntityTypeCode), getter);
        }

        /// <summary>
        /// Clears all items in the custom entity cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears all data for a specific custom entity
        /// </summary>
        /// <param name="customEntityTypeCode">Definition code of the custom entity type</param>
        /// <param name="customEntityId">Id of the custom entity to clear</param>
        public void Clear(string customEntityTypeCode, int customEntityId)
        {
            ClearRoutes(customEntityTypeCode);
        }

        /// <summary>
        /// Clears all cached CustomEntityRoute objects for the specifried custom
        /// entity type
        /// </summary>
        /// <param name="customEntityTypeCode">Definition code of the custom entity type to run</param>
        public void ClearRoutes(string customEntityTypeCode)
        {
            _cache.Clear(GetEntityTypeRoutesCacheKey(customEntityTypeCode));
        }

        private string GetEntityTypeRoutesCacheKey(string customEntityTypeCode)
        {
            return ROUTES_CACHEKEY + customEntityTypeCode;
        }
    }
}
