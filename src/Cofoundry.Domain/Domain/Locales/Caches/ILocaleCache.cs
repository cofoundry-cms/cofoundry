using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
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
        ICollection<ActiveLocale> GetOrAdd(Func<ICollection<ActiveLocale>> getter);

        /// <summary>
        /// Gets all active locales if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the locales aren't in the cache</param>
        Task<ICollection<ActiveLocale>> GetOrAddAsync(Func<Task<ICollection<ActiveLocale>>> getter);
    }
}
