using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Caching
{
    /// <summary>
    /// Factory for creating IObjectCache instances
    /// </summary>
    public interface IObjectCacheFactory
    {
        /// <summary>
        /// Gets an instance of an IObjectCache
        /// </summary>
        /// <param name="cacheNamespace">The cache namespace to organise cache entries under</param>
        /// <returns>IObjectCache instance</returns>
        IObjectCache Get(string cacheNamespace);

        /// <summary>
        /// Clears all object caches created with the factory of all data
        /// </summary>
        void Clear();
    }
}
