using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Cache for locale data, which is frequently requested to 
    /// work out routing
    /// </summary>
    public class LocaleCache : ILocaleCache
    {
        private const string CACHEKEY = "COF_Locales";
        private const string ACTIVELOCALE_CACHEKEY = "ActiveLocales";

        private readonly IObjectCache _cache;
        public LocaleCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        /// <summary>
        /// Gets all active locales if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the locales aren't in the cache</param>
        public ICollection<ActiveLocale> GetOrAdd(Func<ICollection<ActiveLocale>> getter)
        {
            return _cache.GetOrAdd(ACTIVELOCALE_CACHEKEY, getter);
        }

        /// <summary>
        /// Gets all active locales if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the locales aren't in the cache</param>
        public async Task<ICollection<ActiveLocale>> GetOrAddAsync(Func<Task<ICollection<ActiveLocale>>> getter)
        {
            return await _cache.GetOrAddAsync(ACTIVELOCALE_CACHEKEY, getter);
        }
    }
}
