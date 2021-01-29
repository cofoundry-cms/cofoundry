using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to search for custom entity data, returning paged lists of data.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntitySearchQueryBuilder
        : IContentRepositoryCustomEntitySearchQueryBuilder
    {
        /// <summary>
        /// A custom entity search that is not influenced by publish status. It returns 
        /// basic custom entity information with publish status and model data for the
        /// latest version. Designed to be used in the admin panel and not in a 
        /// version-sensitive context sach as a public webpage.
        /// </summary>
        /// <param name="query">Criteria to filter results by.</param>
        /// <returns>Paged set of results.</returns>
        IDomainRepositoryQueryContext<PagedQueryResult<CustomEntitySummary>> AsSummaries(SearchCustomEntitySummariesQuery query);
    }
}
