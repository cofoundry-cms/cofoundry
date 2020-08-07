using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving custom entity page data by a url path/route.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityByPathQueryBuilder
    {
        /// <summary>
        /// Looks up a route for a custom entity page using either the 
        /// CustomEntityId or UrlSlug property. These route objects are 
        /// cached in order to make routing lookups speedy.
        /// </summary>
        /// <param name="query">The query parameters.</param>
        IDomainRepositoryQueryContext<CustomEntityRoute> AsCustomEntityRoute(GetCustomEntityRouteByPathQuery query);
    }
}
