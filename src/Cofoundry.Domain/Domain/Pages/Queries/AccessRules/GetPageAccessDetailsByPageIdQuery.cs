using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all access rules associated with a page, including those inherited from
    /// parent directories.
    /// </summary>>
    public class GetPageAccessDetailsByPageIdQuery : IQuery<PageAccessDetails>
    {
        public GetPageAccessDetailsByPageIdQuery() { }

        /// <summary>
        /// Initializes the query with the specified <paramref name="pageId"/>.
        /// </summary>
        /// <param name="pageId">
        /// Database id of the page to filter access rules to.
        /// </param>
        public GetPageAccessDetailsByPageIdQuery(int pageId)
        {
            PageId = pageId;
        }

        /// <summary>
        /// Database id of the page to filter access rules to.
        /// </summary>
        public int PageId { get; set; }
    }
}
