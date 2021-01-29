using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to search for page entity data, returning paged lists of data.
    /// </summary>
    public interface IContentRepositoryPageSearchQueryBuilder
    {
        /// <summary>
        /// Search page data returning the PageRenderSummary projection, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the PublishStatus query property.
        /// </summary>
        /// <param name="query">Criteria to filter results by.</param>
        /// <returns>Paged set of results.</returns>
        IDomainRepositoryQueryContext<PagedQueryResult<PageRenderSummary>> AsRenderSummaries(SearchPageRenderSummariesQuery query);
    }
}
