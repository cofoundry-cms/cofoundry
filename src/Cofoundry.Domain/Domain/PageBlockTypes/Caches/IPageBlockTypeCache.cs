using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for page block data, which is frequently requested to 
    /// when rendering pages and does no change once the application
    /// is running
    /// </summary>
    public interface IPageBlockTypeCache
    {
        /// <summary>
        /// Gets all page block types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page block types aren't in the cache</param>
        ICollection<PageBlockTypeSummary> GetOrAdd(Func<ICollection<PageBlockTypeSummary>> getter);

        /// <summary>
        /// Gets all page block types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page block types aren't in the cache</param>
        Task<ICollection<PageBlockTypeSummary>> GetOrAddAsync(Func<Task<ICollection<PageBlockTypeSummary>>> getter);

        /// <summary>
        /// Gets all a collection of all PageBlockTypeFileLocation objects if they are already 
        /// cached, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page block types aren't in the cache</param>
        Dictionary<string, PageBlockTypeFileLocation> GetOrAddFileLocations(Func<Dictionary<string, PageBlockTypeFileLocation>> getter);

        /// <summary>
        /// Removes all block type data from the cache
        /// </summary>
        void Clear();
    }
}
