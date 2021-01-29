using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Cache for page block data, which is frequently requested to 
    /// when rendering pages and does no change once the application
    /// is running
    /// </summary>
    public class PageBlockTypeCache : IPageBlockTypeCache
    {
        private const string SUMMARIES_CACHEKEY = "Summaries";
        private const string FILE_LOCATIONS_CACHEKEY = "FileLocations";
        private const string CACHEKEY = "Cofoundry.Domain.PageBlockTypeCache";

        private readonly IObjectCache _cache;

        public PageBlockTypeCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        /// <summary>
        /// Gets all page block types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page block types aren't in the cache</param>
        public ICollection<PageBlockTypeSummary> GetOrAdd(Func<ICollection<PageBlockTypeSummary>> getter)
        {
            return _cache.GetOrAdd(SUMMARIES_CACHEKEY, getter);
        }

        /// <summary>
        /// Gets all page block types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page block types aren't in the cache</param>
        public Task<ICollection<PageBlockTypeSummary>> GetOrAddAsync(Func<Task<ICollection<PageBlockTypeSummary>>> getter)
        {
            return _cache.GetOrAddAsync(SUMMARIES_CACHEKEY, getter);
        }

        /// <summary>
        /// Gets all a collection of all PageBlockTypeFileLocation objects if they are already 
        /// cached, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page blocks types aren't in the cache</param>
        public Dictionary<string, PageBlockTypeFileLocation> GetOrAddFileLocations(Func<Dictionary<string, PageBlockTypeFileLocation>> getter)
        {
            return _cache.GetOrAdd(FILE_LOCATIONS_CACHEKEY, getter);
        }

        /// <summary>
        /// Removes all block type data from the cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }
    }
}
