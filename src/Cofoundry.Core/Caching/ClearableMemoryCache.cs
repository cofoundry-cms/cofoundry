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
    /// A wrapper around MemoryCache to support clearing of all cache entries as
    /// well as sub-sets e.g. by cache namespace.
    /// </summary>
    /// <remarks>
    /// Hopefully this will be addressed in the framework at some point, see 
    /// https://github.com/aspnet/Caching/issues/96
    /// </remarks>
    public class ClearableMemoryCache : IDisposable
    {
        private readonly MemoryCache _memoryCache = null;
        private readonly HashSet<string> _keys = new HashSet<string>();
        private readonly object _lock = new object();

        public ClearableMemoryCache(
            IOptions<MemoryCacheOptions> optionsAccessor
            )
        {
            _memoryCache = new MemoryCache(optionsAccessor);
        }

        public T Get<T>(string key)
        {
            var entry = _memoryCache.Get<T>(key);
            return entry;
        }

        public T GetOrAdd<T>(string key, Func<T> getter, DateTimeOffset? expiry = null)
        {
            T entry;

            if (_memoryCache.TryGetValue<T>(key, out entry))
            {
                return entry;
            }
            else
            {
                lock (_lock)
                {
                    // No need to double lock, we're just making sure 
                    // _keys and _memoryCache have consistent state
                    if (!_keys.Contains(key))
                    {
                        _keys.Add(key);
                    }

                    entry = _memoryCache.GetOrCreate(key, e =>
                    {
                        e.AbsoluteExpiration = expiry;
                        return getter();
                    });
                }

                return entry;
            }
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getter, DateTimeOffset? expiry = null)
        {
            T existingEntry;

            if (_memoryCache.TryGetValue<T>(key, out existingEntry))
            {
                return Task.FromResult(existingEntry);
            }
            else
            {
                Task<T> newEntry;

                lock (_lock)
                {
                    if (!_keys.Contains(key))
                    {
                        _keys.Add(key);

                    }
                    newEntry = _memoryCache.GetOrCreateAsync(key, e =>
                    {
                        e.AbsoluteExpiration = expiry;
                        return getter();
                    });
                }
            
                return newEntry;
            }
        }

        public void ClearAll(string cacheNamespace = null)
        {
            List<string> listToClear;

            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(cacheNamespace))
                {
                    listToClear = _keys.ToList();
                }
                else
                {
                    listToClear = _keys
                        .Where(k => k.StartsWith(cacheNamespace))
                        .ToList();
                }

                foreach (var key in listToClear)
                {
                    _keys.Remove(key);
                    _memoryCache.Remove(key);
                }
            }
        }

        public void ClearEntry(string key = null)
        {
            lock (_lock)
            {
                _keys.Remove(key);
                _memoryCache.Remove(key);
            }
        }

        public void Dispose()
        {
            if (_memoryCache != null)
            {
                _memoryCache.Dispose();
            }
        }
    }
}
