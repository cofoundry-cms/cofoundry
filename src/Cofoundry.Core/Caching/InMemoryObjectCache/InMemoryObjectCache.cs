using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching.Internal
{
    /// <summary>
    /// Simple typed caching stored in memory.
    /// </summary>
    public class InMemoryObjectCache : IObjectCache
    {
        #region constructor

        private string _cacheNamespace;
        private ClearableMemoryCache _cache;

        /// <summary>
        /// Creates a new instance of an InMemoryObjectCache object.
        /// </summary>
        /// <param name="cache">The parent MemoryCache instance to use.</param>
        /// <param name="cacheNamespace">Unique cache namespace to organise all cache items under.</param>
        public InMemoryObjectCache(ClearableMemoryCache cache, string cacheNamespace)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));

            _cache = cache;
            _cacheNamespace = cacheNamespace ?? string.Empty;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets an object stored in the cache, returning null if it
        /// does not exist.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="key">Unique key of the cache entry</param>
        /// <returns>The value of the cached object if it exists; otherwise null.</returns>
        public T Get<T>(string key)
        {
            var fullKey = CreateCacheKey(key);
            var entry = _cache.Get<T>(fullKey);
            return entry;
        }

        /// <summary>
        /// Gets an entry from the cache or creates and interts a new item, into the cache 
        /// if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="key">Unique key of the cache entry</param>
        /// <param name="getter">A function that returns an entry to insert into the cache if it is empty</param>
        /// <param name="expiry">An optional absolute expiry time of the cache entry.</param>
        /// <returns>The value of the cache entry if it exists; otherwise the result of the getter function.</returns>
        public T GetOrAdd<T>(string key, Func<T> getter, DateTimeOffset? expiry = null)
        {
            var fullKey = CreateCacheKey(key);
            var item = _cache.GetOrAdd<T>(fullKey, getter, expiry);

            return item;
        }

        /// <summary>
        /// Gets an entry from the cache or creates and interts a new item, into the cache 
        /// if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="key">Unique key of the cache entry</param>
        /// <param name="getter">An async function that returns an entry to insert into the cache if it is empty</param>
        /// <param name="expiry">An optional absolute expiry time of the cache entry.</param>
        /// <returns>The value of the cache entry if it exists; otherwise the result of the getter function.</returns>
        public Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getter, DateTimeOffset? expiry = null)
        {
            var fullKey = CreateCacheKey(key);
            var item = _cache.GetOrAddAsync<T>(fullKey, getter, expiry);

            return item;
        }

        /// <summary>
        /// Clears the specified cache entry. If the key parameter is not provided, all
        /// entries in the cache namespace are removed.
        /// </summary>
        /// <param name="key">Unique key of the cache entry to update</param>
        public void Clear(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _cache.ClearAll(CreateCacheKey(string.Empty));
            }
            else
            {
                _cache.ClearEntry(CreateCacheKey(key));
            }
        }

        #endregion

        #region helpers

        private string CreateCacheKey(string key)
        {
            return _cacheNamespace + ":" + key;
        }

        #endregion
    }
}
