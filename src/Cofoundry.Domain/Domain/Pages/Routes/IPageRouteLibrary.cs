using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Route library for pages
    /// </summary>
    public interface IPageRouteLibrary
    {
        /// <summary>
        /// Simple but less efficient way of getting a page url if you only know 
        /// the id. Use the overload accepting an IPageRoute if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        Task<string> PageAsync(int? pageId);

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        string Page(IPageRoute route);

        /// <summary>
        /// Gets the full (relative) url of a custom entity details page
        /// </summary>
        string Page(ICustomEntityRoutable customEntity);
    }
}
