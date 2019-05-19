using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns a collection of content managed regions with
    /// block data for a specific version of a page.
    /// </summary>
    public class GetPageRegionDetailsByPageVersionIdQuery : IQuery<ICollection<PageRegionDetails>>
    {
        /// <summary>
        /// Returns a collection of the content managed regions and
        /// blocks for a specific version of a page.
        /// </summary>
        public GetPageRegionDetailsByPageVersionIdQuery() { }

        /// <summary>
        /// Returns a collection of the content managed regions and
        /// blocks for a specific version of a page.
        /// </summary>
        /// <param name="pageVersionId">Database id of the page version to get content data for.</param>
        public GetPageRegionDetailsByPageVersionIdQuery(int pageVersionId)
        {
            PageVersionId = pageVersionId;
        }

        /// <summary>
        /// Database id of the page version to get content data for.
        /// </summary>
        public int PageVersionId { get; set; }
    }
}
