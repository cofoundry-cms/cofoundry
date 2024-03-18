﻿namespace Cofoundry.Domain;

/// <summary>
/// Cache for locale data, which is frequently requested to 
/// work out routing
/// </summary>
public interface ILocaleCache
{
    /// <summary>
    /// Gets all active locales if they are already cached, otherwise the 
    /// getter is invoked and the result is cached and returned
    /// </summary>
    /// <param name="getter">Function to invoke if the locales aren't in the cache</param>
    Task<IReadOnlyCollection<ActiveLocale>> GetOrAddAsync(Func<Task<IReadOnlyCollection<ActiveLocale>>> getter);
}
