using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving paged collections of page versions by a page id.
    /// </summary>
    public interface IAdvancedContentRepositoryPageVersionsByPageIdQueryBuilder
    {
        /// <summary>
        /// Returns a paged collection of versions of a specific page, ordered 
        /// historically with the latest/draft version first.
        /// </summary>
        Task<PagedQueryResult<PageVersionSummary>> AsVersionSummariesAsync(GetPageVersionSummariesByPageIdQuery query);
    }
}
