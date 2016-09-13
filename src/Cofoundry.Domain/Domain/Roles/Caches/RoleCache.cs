using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RoleCache : IRoleCache
    {
        #region constructor

        private const string ROLE_DETAILS_CACHEKEY = "RoleDetails";
        private const string ANON_ROLE_CACHEKEY = "AnonymousRole";
        private const string CACHEKEY = "COF_Roles";

        private readonly IObjectCache _cache;

        public RoleCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        public RoleDetails GetOrAdd(int roleId, Func<RoleDetails> getter)
        {
            return _cache.GetOrAdd(ROLE_DETAILS_CACHEKEY + roleId, getter);
        }

        public async Task<RoleDetails> GetOrAddAsync(int roleId, Func<Task<RoleDetails>> getter)
        {
            return await _cache.GetOrAddAsync(ROLE_DETAILS_CACHEKEY + roleId, getter);
        }

        public RoleDetails GetOrAddAnonymousRole(Func<RoleDetails> getter)
        {
            return _cache.GetOrAdd(ANON_ROLE_CACHEKEY, getter);
        }

        public async Task<RoleDetails> GetOrAddAnonymousRoleAsync(Func<Task<RoleDetails>> getter)
        {
            return await _cache.GetOrAddAsync(ANON_ROLE_CACHEKEY, getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears all data relating to a specific page
        /// </summary>
        public void Clear(int roleId)
        {
            _cache.Clear(ANON_ROLE_CACHEKEY);
            _cache.Clear(ROLE_DETAILS_CACHEKEY + roleId);
        }

        #endregion
    }
}
