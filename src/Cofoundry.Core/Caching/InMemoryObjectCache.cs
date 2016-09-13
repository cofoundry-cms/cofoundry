using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching
{
    /// <summary>
    /// Simple typed caching stored in memory.
    /// </summary>
    public class InMemoryObjectCache : IObjectCache
    {
        #region constructor

        private string _cacheNamespace;
        private MemoryCache _cache;

        /// <summary>
        /// Creates a new instance of an InMemoryObjectCache object.
        /// </summary>
        /// <param name="cache">The parent MemoryCache instance to use.</param>
        /// <param name="cacheNamespace">Unique cache namespace to organise all cache items under.</param>
        public InMemoryObjectCache(MemoryCache cache, string cacheNamespace)
        {
            Condition.Requires(cache).IsNotNull();

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
            var o = _cache.Get(CreateCacheKey(key));
            if (o == null) return default(T);
            return (T)o;
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
            var item = Get<T>(key);
            if (item == null)
            {
                item = getter();
                Put(key, item, expiry);
            }

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
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getter, DateTimeOffset? expiry = null)
        {
            var item = Get<T>(key);
            if (item == null)
            {
                item = await getter();
                Put(key, item, expiry);
            }

            return item;
        }

        /// <summary>
        /// Adds or updates the cache entry with the specified key.
        /// </summary>
        /// <param name="key">Unique key of the cache entry</param>
        /// <param name="item">object to add/update to the cache</param>
        /// <param name="expiry">An optional absolute expiry time of the cache entry.</param>
        public void Put<T>(string key, T item, DateTimeOffset? expiry = null)
        {
            if (item == null)
            {
                Clear(key);
                return;
            }
            CacheItemPolicy policy = null;

            if (expiry.HasValue)
            {
                policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = expiry.Value;

            }

            _cache.Set(CreateCacheKey(key), item, policy);
        }

        /// <summary>
        /// Clears the specified cache entry. If the key parameter is not provided, all
        /// entries in the cache namespace are removed.
        /// </summary>
        /// <param name="key">Unique key of the cache entry to update</param>
        public void Clear(string key = null)
        {
            string[] keys;

            if (key == null)
            {
                keys = _cache
                    .Where(i => i.Key.StartsWith(_cacheNamespace + ":"))
                    .Select(i => i.Key)
                    .ToArray();
            }
            else
            {
                keys = new string[] { CreateCacheKey(key) };
            }

            foreach (var k in keys)
            {
                _cache.Remove(k);
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
