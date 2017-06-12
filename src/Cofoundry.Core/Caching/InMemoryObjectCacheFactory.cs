using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching
{
    /// <summary>
    /// Factory for creating IObjectCache instances
    /// </summary>
    public class InMemoryObjectCacheFactory : IObjectCacheFactory
    {
        #region constructor

        private readonly ClearableMemoryCache _memoryCache;

        /// <summary>
        /// Creates a new InMemoryObjectCacheFactory instance
        /// </summary>
        /// <param name="cacheName">
        /// The name of the MemoryCache configuration to use. If the cacheName is null the Default 
        /// MemoryCache instance is used (recommended).
        /// </param>
        public InMemoryObjectCacheFactory(
            IOptions<MemoryCacheOptions> optionsAccessor
            )
        {
            _memoryCache = new ClearableMemoryCache(optionsAccessor);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets an instance of an IObjectCache
        /// </summary>
        /// <param name="cacheNamespace">The cache namespace to organise cache entries under</param>
        /// <returns>IObjectCache instance</returns>
        public IObjectCache Get(string cacheNamespace)
        {
            return new InMemoryObjectCache(_memoryCache, cacheNamespace);
        }

        /// <summary>
        /// Clears all object caches created with the factory of all data
        /// </summary>
        public void Clear()
        {
            _memoryCache.ClearAll();
        }

        #endregion
    }
}
