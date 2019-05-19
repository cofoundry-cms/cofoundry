using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retieving data for "not found" (404) pages.
    /// </summary>
    public interface IAdvancedContentRepositoryPageNotFoundQueryBuilder
    {
        /// <summary>
        /// Attempts to find the most relevant 'Not Found' page route by searching
        /// for a 'Not Found' page up the directory tree of a specific path.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        Task<PageRoute> GetByPathAsync(GetNotFoundPageRouteByPathQuery query);
    }
}
