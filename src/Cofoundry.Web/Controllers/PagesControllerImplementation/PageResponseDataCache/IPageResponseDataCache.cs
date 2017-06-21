using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Caches the data used to render dynamic pages for the duration of the 
    /// request so it can be accessed outside of the controller and view. 
    /// Primarily used to render the siteviewer via SiteViewerContentFilterAttribute.
    /// </summary>
    public interface IPageResponseDataCache
    {
        /// <summary>
        /// Sets the cache value.
        /// </summary>
        void Set(IPageResponseData data);

        /// <summary>
        /// Gets the cache value or null if it has not been set.
        /// </summary>
        IPageResponseData Get();
    }
}