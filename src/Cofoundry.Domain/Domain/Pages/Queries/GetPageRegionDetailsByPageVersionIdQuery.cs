using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a collection of the content managed regions and
    /// blocks for a specific version of a page. These are the 
    /// content blocks that get rendered in the page template linked
    /// to the page version.
    /// </summary>
    public class GetPageRegionDetailsByPageVersionIdQuery : IQuery<IEnumerable<PageRegionDetails>>
    {
        public GetPageRegionDetailsByPageVersionIdQuery() { }

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
