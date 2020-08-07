using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to search for role data, returning paged lists of data.
    /// </summary>
    public interface IContentRepositoryRoleSearchQueryBuilder
    {
        /// <summary>
        /// Seaches roles based on simple filter criteria and return 
        /// a paged result. 
        /// </summary>
        /// <param name="query">Criteria to filter results by.</param>
        IDomainRepositoryQueryContext<PagedQueryResult<RoleMicroSummary>> AsMicroSummaries(SearchRolesQuery query);
    }
}
