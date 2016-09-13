using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class SettingCache : ISettingCache
    {
        #region constructor

        private const string CACHEKEY = "COF_Settings";
        private const string SETTING_TABLE_KEY = "SettingTable";

        private readonly IObjectCache _cache;
        public SettingCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        public Dictionary<string, string> GetOrAddSettingsTable(Func<Dictionary<string, string>> getter)
        {
            return _cache.GetOrAdd(SETTING_TABLE_KEY, getter);
        }

        public async Task<Dictionary<string, string>> GetOrAddSettingsTableAsync(Func<Task<Dictionary<string, string>>> getter)
        {
            return await _cache.GetOrAddAsync(SETTING_TABLE_KEY, getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        #endregion
    }
}
