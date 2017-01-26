using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for page module data, which is frequently requested to 
    /// when rendering pages and does no change once the application
    /// is running
    /// </summary>
    public class PageModuleTypeCache : IPageModuleTypeCache
    {
        #region constructor

        private const string SUMMARIES_CACHEKEY = "Summaries";
        private const string FILE_LOCATIONS_CACHEKEY = "FileLocations";
        private const string CACHEKEY = "Cofoundry.Domain.PageModuleTypeCache";

        private readonly IObjectCache _cache;

        public PageModuleTypeCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets all page module types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page module types aren't in the cache</param>
        public PageModuleTypeSummary[] GetOrAdd(Func<PageModuleTypeSummary[]> getter)
        {
            return _cache.GetOrAdd(SUMMARIES_CACHEKEY, getter);
        }

        /// <summary>
        /// Gets all page module types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page module types aren't in the cache</param>
        public Task<PageModuleTypeSummary[]> GetOrAddAsync(Func<Task<PageModuleTypeSummary[]>> getter)
        {
            return _cache.GetOrAddAsync(SUMMARIES_CACHEKEY, getter);
        }

        /// <summary>
        /// Gets all a collection of all PageModuleTypeFileLocation objects if they are already 
        /// cached, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page module types aren't in the cache</param>
        public Dictionary<string, PageModuleTypeFileLocation> GetOrAddFileLocations(Func<Dictionary<string, PageModuleTypeFileLocation>> getter)
        {
            return _cache.GetOrAdd(FILE_LOCATIONS_CACHEKEY, getter);
        }

        /// <summary>
        /// Removes all module type data from the cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        #endregion
    }
}
