﻿namespace Cofoundry.Core.Caching.Internal;

/// <summary>
/// Implementation to use when caching is disabled completedly.
/// </summary>
public class CacheDisabledObjectCache : IObjectCache
{
    public void Clear(string? key = null)
    {
    }

    public T? Get<T>(string key)
    {
        return default(T);
    }

    public T? GetOrAdd<T>(string key, Func<T> getter, DateTimeOffset? expiry = null)
    {
        return getter();
    }

    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> getter, DateTimeOffset? expiry = null)
    {
        var result = await getter();
        return result;
    }
}
