using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns a paged collection of versions of a specific page, ordered 
    /// historically with the latest/draft version first.
    /// </summary>
    public class GetPageVersionSummariesByPageIdQuery 
        : SimplePageableQuery
        , IQuery<PagedQueryResult<PageVersionSummary>>
    {
        /// <summary>
        /// Returns all versions of a specific page, ordered historically with
        /// the latest/draft version first.
        /// </summary>
        public GetPageVersionSummariesByPageIdQuery()
        {
        }

        /// <summary>
        /// Returns all versions of a specific page, ordered historically with
        /// the latest/draft version first.
        /// </summary>
        /// <param name="pageId">Database id of the page to get versions for.</param>
        public GetPageVersionSummariesByPageIdQuery(int pageId)
        {
            PageId = pageId;
        }

        /// <summary>
        /// Database id of the page to get versions for.
        /// </summary>
        public int PageId { get; set; }
    }
}
