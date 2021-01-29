using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving data for "not found" (404) pages.
    /// </summary>
    public interface IAdvancedContentRepositoryPageNotFoundQueryBuilder
    {
        /// <summary>
        /// A query that attempts to find the most relevant 'Not Found' page route by searching
        /// for a 'Not Found' page up the directory tree of a specific path.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<PageRoute> GetByPath(GetNotFoundPageRouteByPathQuery query);
    }
}
