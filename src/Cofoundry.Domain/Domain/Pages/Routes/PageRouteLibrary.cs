using Cofoundry.Core;
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
        public async Task<string> PageAsync(int? pageId)
        {
            if (!pageId.HasValue) return string.Empty;

            var query = new GetPageRouteByIdQuery(pageId.Value);
            var route = await _queryExecutor.ExecuteAsync(query);

            return Page(route);
        }

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        public string Page(IPageRoute route)
        {
            if (route == null) return string.Empty;
            return route.FullPath;
        }

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        public string Page(PageRoutingInfo route)
        {
            if (route == null) return string.Empty;

            if (route.CustomEntityRouteRule != null && route.CustomEntityRoute != null)
            {
                return route.CustomEntityRouteRule.MakeUrl(route.PageRoute, route.CustomEntityRoute);
            }

            return Page(route.PageRoute);
        }

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        public string Page(ICustomEntityRoutable customEntity)
        {
            if (customEntity == null || EnumerableHelper.IsNullOrEmpty(customEntity.PageUrls)) return string.Empty;

            // Multiple details routes are technically possible, but
            // shouldn't really happen and if they are then it's reasonable
            // to expect someone to construct the routes manually themselves.
            var route = customEntity
                .PageUrls
                .OrderBy(r => r.Length)
                .FirstOrDefault();
            
            return route;
        }
    }
}
