using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for image asset data, which is frequently accessed when
    /// running requests in order to work out permissions
    /// </summary>
    public interface IRoleCache
    {
        /// <summary>
        /// Gets a dictionary used to lookup role ids from role codes. This is used
        /// because roles are cached by id rather than by code.
        /// </summary>
        /// <param name="getter">Function to invoke if the lookup isn't in the cache</param>
        Task<ReadOnlyDictionary<string, int>> GetOrAddRoleCodeLookupAsync(Func<Task<ReadOnlyDictionary<string, int>>> getter);

        /// <summary>
        /// Gets a role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="roleId">Id of the role to return</param>
        /// <param name="getter">Function to invoke if the role isn't in the cache</param>
        RoleDetails GetOrAdd(int roleId, Func<RoleDetails> getter);

        /// <summary>
        /// Gets a role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="roleId">Id of the role to return</param>
        /// <param name="getter">Function to invoke if the role isn't in the cache</param>
        Task<RoleDetails> GetOrAddAsync(int roleId, Func<Task<RoleDetails>> getter);

        /// <summary>
        /// Gets the anonnymous role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the annonymous role isn't in the cache</param>
        RoleDetails GetOrAddAnonymousRole(Func<RoleDetails> getter);

        /// <summary>
        /// Gets the anonnymous role if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the annonymous role isn't in the cache</param>
        Task<RoleDetails> GetOrAddAnonymousRoleAsync(Func<Task<RoleDetails>> getter);

        /// <summary>
        /// Clears all items in the role cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears the specified cache entry. If the key parameter is not provided, all
        /// entries in the cache namespace are removed.
        /// </summary>
        /// <param name="roleId">Id of the role to clear out all cache entries for</param>
        void Clear(int roleId);
    }
}
