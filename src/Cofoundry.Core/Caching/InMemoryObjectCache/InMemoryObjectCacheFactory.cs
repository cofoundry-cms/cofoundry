using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching.Internal
{
    /// <summary>
    /// Factory for creating IObjectCache instances.
    /// </summary>
    public class InMemoryObjectCacheFactory : IObjectCacheFactory
    {
        private readonly ClearableMemoryCache _memoryCache;
        private readonly InMemoryObjectCacheSettings _inMemoryObjectCacheSettings;

        public InMemoryObjectCacheFactory(
            IOptions<MemoryCacheOptions> optionsAccessor,
            InMemoryObjectCacheSettings inMemoryObjectCacheSettings
            )
        {
            _memoryCache = new ClearableMemoryCache(optionsAccessor);
            _inMemoryObjectCacheSettings = inMemoryObjectCacheSettings;
        }

        /// <summary>
        /// Gets an instance of an IObjectCache.
        /// </summary>
        /// <param name="cacheNamespace">The cache namespace to organise cache entries under</param>
        /// <returns>IObjectCache instance</returns>
        public IObjectCache Get(string cacheNamespace)
        {
            if (_inMemoryObjectCacheSettings.CacheMode == InMemoryObjectCacheMode.Off)
            {
                return new CacheDisabledObjectCache();
            }
            else
            {
                return new InMemoryObjectCache(_memoryCache, cacheNamespace);
            }
        }

        /// <summary>
        /// Clears all object caches created with the factory of all data
        /// </summary>
        public void Clear()
        {
            _memoryCache.ClearAll();
        }
    }
}
