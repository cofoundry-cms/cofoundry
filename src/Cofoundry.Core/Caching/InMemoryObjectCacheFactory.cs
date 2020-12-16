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

        public InMemoryObjectCacheFactory(
            IOptions<MemoryCacheOptions> optionsAccessor
            )
        {
            _memoryCache = new ClearableMemoryCache(optionsAccessor);
        }

        /// <summary>
        /// Gets an instance of an IObjectCache.
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
    }
}
