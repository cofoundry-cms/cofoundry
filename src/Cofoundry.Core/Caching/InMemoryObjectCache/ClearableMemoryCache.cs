using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Cofoundry.Core.Caching.Internal;

/// <summary>
/// A wrapper around MemoryCache to support clearing of all cache entries as
/// well as sub-sets e.g. by cache namespace.
/// </summary>
/// <remarks>
/// Originally this  was intended to temporarily replace <see cref="MemoryCache"/> 
/// which did  not support clearing, see https://github.com/aspnet/Caching/issues/96.
/// Although clearing is now supported, this class is still useful for clearing based
/// on namespaces.
/// </remarks>
public class ClearableMemoryCache : IDisposable
{
    private readonly MemoryCache _memoryCache;
    private readonly HashSet<string> _keys = [];
    private readonly Lock _lock = new();

    /// <summary>
    /// DI constructor.
    /// </summary>
    public ClearableMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor)
    {
        _memoryCache = new MemoryCache(optionsAccessor);
    }

    /// <summary>
    /// Gets the value associated with this key if present.
    /// </summary>
    /// <typeparam name="TItem">The type of the object to get.</typeparam>
    /// <param name="key">The key of the value to get.</param>
    public TItem? Get<TItem>(string key)
    {
        var entry = _memoryCache.Get<TItem>(key);
        return entry;
    }

    public TItem? GetOrAdd<TItem>(string key, Func<TItem> getter, DateTimeOffset? expiry = null)
    {
        if (_memoryCache.TryGetValue(key, out TItem? entry))
        {
            return entry;
        }
        else
        {
            lock (_lock)
            {
                // No need to double lock, we're just making sure 
                // _keys and _memoryCache have consistent state
                _keys.Add(key);

                entry = _memoryCache.GetOrCreate(key, e =>
                {
                    e.AbsoluteExpiration = expiry;
                    return getter();
                });
            }

            return entry;
        }
    }

    public Task<TItem?> GetOrAddAsync<TItem>(string key, Func<Task<TItem>> getter, DateTimeOffset? expiry = null)
    {
        if (_memoryCache.TryGetValue(key, out TItem? existingEntry))
        {
            return Task.FromResult(existingEntry);
        }
        else
        {
            Task<TItem?> newEntry;

            lock (_lock)
            {
                _keys.Add(key);
                newEntry = _memoryCache.GetOrCreateAsync(key, e =>
                {
                    e.AbsoluteExpiration = expiry;
                    return getter();
                });
            }

            return newEntry;
        }
    }

    public void ClearAll(string? cacheNamespace = null)
    {
        lock (_lock)
        {
            if (string.IsNullOrWhiteSpace(cacheNamespace))
            {
                _keys.Clear();
                _memoryCache.Clear();
                return;
            }

            var listToClear = _keys
                .Where(k => k.StartsWith(cacheNamespace))
                .ToList();

            foreach (var key in listToClear)
            {
                _keys.Remove(key);
                _memoryCache.Remove(key);
            }
        }
    }

    /// <summary>
    /// Removes the object from the the cache with the associated <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the object to remove.</param>
    public void ClearEntry(string key)
    {
        lock (_lock)
        {
            _keys.Remove(key);
            _memoryCache.Remove(key);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _memoryCache?.Dispose();
    }
}
