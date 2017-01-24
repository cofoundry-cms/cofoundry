using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for page module data, which is frequently requested to 
    /// when rendering pages and does no change once the application
    /// is running
    /// </summary>
    public interface IPageModuleTypeCache
    {
        /// <summary>
        /// Gets all page module types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page module types aren't in the cache</param>
        PageModuleTypeSummary[] GetOrAdd(Func<PageModuleTypeSummary[]> getter);

        /// <summary>
        /// Gets all page module types if they are already cached, otherwise the 
        /// getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page module types aren't in the cache</param>
        Task<PageModuleTypeSummary[]> GetOrAddAsync(Func<Task<PageModuleTypeSummary[]>> getter);

        /// <summary>
        /// Gets all a collection of all PageModuleTypeFileLocation objects if they are already 
        /// cached, otherwise the getter is invoked and the result is cached and returned
        /// </summary>
        /// <param name="getter">Function to invoke if the page module types aren't in the cache</param>
        Dictionary<string, PageModuleTypeFileLocation> GetOrAddFileLocations(Func<Dictionary<string, PageModuleTypeFileLocation>> getter);
        
        /// <summary>
        /// Removes all module type data from the cache
        /// </summary>
        void Clear();
    }
}
