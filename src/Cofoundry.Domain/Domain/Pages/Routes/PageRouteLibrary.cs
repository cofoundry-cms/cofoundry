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
    public class PageRouteLibrary : IPageRouteLibrary
    {
        private readonly IQueryExecutor _queryExecutor;

        public PageRouteLibrary(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        /// <summary>
        /// Simple but less efficient way of getting a page url if you only know 
        /// the id. Use the overload accepting an IPageRoute if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        public string Page(int? pageId)
        {
            if (!pageId.HasValue) return string.Empty;
            var route = _queryExecutor.GetById<PageRoute>(pageId.Value);
            return Page(route);
        }

        /// <summary>
        /// Gets the full url of a page
        /// </summary>
        public string Page(IPageRoute route)
        {
            if (route == null) return string.Empty;
            return route.FullPath;
        }
    }
}
