using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for custom entity data, particularly CustomEntityRoute objects
    /// which are frequently requested to work out routing
    /// </summary>
    public interface ICustomEntityCache
    {
        /// <summary>
        /// Gets a collection of custom entity routes for the specified
        /// custom entity type. If the collection is already cached it 
        /// is returned, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="customEntityTypeCode">Definition code of the custom entity type to return routes for</param>
        /// <param name="getter">Function to invoke if the custom entities are not in the cache</param>
        Task<ICollection<CustomEntityRoute>> GetOrAddAsync(string customEntityTypeCode, Func<Task<ICollection<CustomEntityRoute>>> getter);

        /// <summary>
        /// Clears all items in the custom entity cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears all data for a specific custom entity
        /// </summary>
        /// <param name="customEntityTypeCode">Definition code of the custom entity type</param>
        /// <param name="customEntityId">Id of the custom entity to clear</param>
        void Clear(string customEntityTypeCode, int customEntityId);

        /// <summary>
        /// Clears all cached CustomEntityRoute objects for the specifried custom
        /// entity type
        /// </summary>
        /// <param name="customEntityTypeCode">Definition code of the custom entity type to run</param>
        void ClearRoutes(string customEntityTypeCode);
    }
}
