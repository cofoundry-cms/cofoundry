using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to search for page entity data, returning paged lists of data.
    /// </summary>
    public interface IAdvancedContentRepositoryPageSearchQueryBuilder
        : IContentRepositoryPageSearchQueryBuilder
    {
        /// <summary>
        /// Search page data returning the PageSummary projection, which is primarily used
        /// to display lists of page information in the admin panel. The query isn't version 
        /// specific and should not be used to render content out to a live page because some of
        /// the pages returned may be unpublished.
        /// </summary>
        /// <param name="query">Criteria to filter results by.</param>
        /// <returns>Paged set of results.</returns>
        IDomainRepositoryQueryContext<PagedQueryResult<PageSummary>> AsSummaries(SearchPageSummariesQuery query);
    }
}
