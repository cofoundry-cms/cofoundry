using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns data for a specific block in a page version by it's id. Because
    /// the mapped display model may contain other versioned entities, you can 
    /// optionally pass down a PublishStatusQuery to use in the mapping process.
    /// </summary>
    public class GetPageVersionBlockRenderDetailsByIdQuery : IQuery<PageVersionBlockRenderDetails>
    {
        public GetPageVersionBlockRenderDetailsByIdQuery() { }

        /// <summary>
        /// Initializes the query with the specified parameters.
        /// </summary>
        /// <param name="pageVersionBlockId">Id of the page block version to find..</param>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        public GetPageVersionBlockRenderDetailsByIdQuery(int pageVersionBlockId, PublishStatusQuery? publishStatus = null)
        {
            PageVersionBlockId = pageVersionBlockId;
            if (publishStatus.HasValue)
            {
                PublishStatus = publishStatus.Value;
            }
        }

        /// <summary>
        /// Id of the page block version to find.
        /// </summary>
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// Optional publish status of the parent page or custom entity 
        /// being mapped. This is provided so related entities can use
        /// the same publish status when queried in the mapping process. 
        /// Defaults to PublishStatusQuery.Published.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }
    }
}
