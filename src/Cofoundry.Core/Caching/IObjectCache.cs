using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching
{
    /// <summary>
    /// <para>
    /// A simple object cache. Note that object caches are only suitable in certain scenarios
    /// typically where the data set is small or limited in scope and changes infrequently.
    /// </para>
    /// <para>
    /// Caches can be volatile and therefore are not guaranteed to persits once set.
    /// </para>
    /// </summary>
    public interface IObjectCache
    {
        /// <summary>
        /// Gets an object stored in the cache, returning null if it
        /// does not exist.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="key">Unique key of the cache entry</param>
        /// <returns>The value of the cached object if it exists; otherwise null.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets an entry from the cache or creates and interts a new item, into the cache 
        /// if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="key">Unique key of the cache entry</param>
        /// <param name="getter">A function that returns an entry to insert into the cache if it is empty</param>
        /// <param name="expiry">An optional absolute expiry time of the cache entry.</param>
        /// <returns>The value of the cache entry if it exists; otherwise the result of the getter function.</returns>
        T GetOrAdd<T>(string key, Func<T> getter, DateTimeOffset? expiry = null);

        /// <summary>
        /// Gets an entry from the cache or creates and interts a new item, into the cache 
        /// if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="key">Unique key of the cache entry</param>
        /// <param name="getter">An async function that returns an entry to insert into the cache if it is empty</param>
        /// <param name="expiry">An optional absolute expiry time of the cache entry.</param>
        /// <returns>The value of the cache entry if it exists; otherwise the result of the getter function.</returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getter, DateTimeOffset? expiry = null);

        /// <summary>
        /// Clears the specified cache entry. If the key parameter is not provided, all
        /// entries in the cache namespace are removed.
        /// </summary>
        /// <param name="key">Unique key of the cache entry to update</param>
        void Clear(string key = null);
    }
}
