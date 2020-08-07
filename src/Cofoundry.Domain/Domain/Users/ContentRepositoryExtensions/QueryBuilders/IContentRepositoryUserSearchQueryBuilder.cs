using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to search for user data, returning paged lists of data.
    /// </summary>
    public interface IContentRepositoryUserSearchQueryBuilder
    {
        /// <summary>
        /// Searches users based on simple filter criteria and returns 
        /// a paged set of summary results. 
        /// </summary>
        /// <param name="query">Criteria to filter results by.</param>
        IDomainRepositoryQueryContext<PagedQueryResult<UserSummary>> AsSummaries(SearchUserSummariesQuery query);
    }
}
