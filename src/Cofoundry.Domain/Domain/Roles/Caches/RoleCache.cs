using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Cache for role data, which is frequently accessed when
    /// running requests in order to work out permissions
    /// </summary>
    public class RoleCache : IRoleCache
    {
        private const string ROLE_CODE_LOOKUP_CACHEKEY = "RoleCodes";
        private const string ROLE_DETAILS_CACHEKEY = "RoleDetails";
        private const string ANON_ROLE_CACHEKEY = "AnonymousRole";
        private const string CACHEKEY = "COF_Roles";

        private readonly IObjectCache _cache;

        public RoleCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        /// <summary>
        /// Gets a dictionary used to lookup role ids from role codes. This is used
        /// because roles are cached by id rather than by code.
        /// </summary>
        /// <param name="getter">Function to invoke if the lookup isn't in the cache</param>
        public Task<ReadOnlyDictionary<string, int>> GetOrAddRoleCodeLookupAsync(Func<Task<ReadOnlyDictionary<string, int>>> getter)
        {
            return _cache.GetOrAddAsync(ROLE_CODE_LOOKUP_CACHEKEY, getter);
        }

        /// <summary>
        /// Gets a role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="roleId">Id of the role to return</param>
        /// <param name="getter">Function to invoke if the role isn't in the cache</param>
        public RoleDetails GetOrAdd(int roleId, Func<RoleDetails> getter)
        {
            return _cache.GetOrAdd(ROLE_DETAILS_CACHEKEY + roleId, getter);
        }

        /// <summary>
        /// Gets a role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="roleId">Id of the role to return</param>
        /// <param name="getter">Function to invoke if the role isn't in the cache</param>
        public Task<RoleDetails> GetOrAddAsync(int roleId, Func<Task<RoleDetails>> getter)
        {
            return _cache.GetOrAddAsync(ROLE_DETAILS_CACHEKEY + roleId, getter);
        }

        /// <summary>
        /// Gets the anonnymous role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the annonymous role isn't in the cache</param>
        public RoleDetails GetOrAddAnonymousRole(Func<RoleDetails> getter)
        {
            return _cache.GetOrAdd(ANON_ROLE_CACHEKEY, getter);
        }

        /// <summary>
        /// Gets the anonnymous role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the annonymous role isn't in the cache</param>
        public Task<RoleDetails> GetOrAddAnonymousRoleAsync(Func<Task<RoleDetails>> getter)
        {
            return _cache.GetOrAddAsync(ANON_ROLE_CACHEKEY, getter);
        }

        /// <summary>
        /// Clears all items in the role cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears the specified cache entry. If the key parameter is not provided, all
        /// entries in the cache namespace are removed.
        /// </summary>
        /// <param name="roleId">Id of the role to clear out all cache entries for</param>
        public void Clear(int roleId)
        {
            var anonymousRole = _cache.Get<RoleDetails>(ANON_ROLE_CACHEKEY);
            if (anonymousRole?.RoleId == roleId)
            {
                _cache.Clear(ANON_ROLE_CACHEKEY);
            }
            _cache.Clear(ROLE_DETAILS_CACHEKEY + roleId);
        }
    }
}
