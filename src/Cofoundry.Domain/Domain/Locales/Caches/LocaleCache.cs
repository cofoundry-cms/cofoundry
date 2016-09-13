using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class LocaleCache : ILocaleCache
    {
        #region constructor

        private const string CACHEKEY = "COF_Locales";
        private const string ACTIVELOCALE_CACHEKEY = "ActiveLocales";

        private readonly IObjectCache _cache;
        public LocaleCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        public ActiveLocale[] GetOrAdd(Func<ActiveLocale[]> getter)
        {
            return _cache.GetOrAdd(ACTIVELOCALE_CACHEKEY, getter);
        }

        public async Task<ActiveLocale[]> GetOrAddAsync(Func<Task<ActiveLocale[]>> getter)
        {
            return await _cache.GetOrAddAsync(ACTIVELOCALE_CACHEKEY, getter);
        }

        #endregion
    }
}
