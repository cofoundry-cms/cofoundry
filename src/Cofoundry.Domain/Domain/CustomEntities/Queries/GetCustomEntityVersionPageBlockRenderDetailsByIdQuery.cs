using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns data for a specific custom entity page block by it's id. Because
    /// the mapped display model may contain other versioned entities, you can 
    /// optionally pass down a PublishStatusQuery to use in the mapping process.
    /// </summary>
    public class GetCustomEntityVersionPageBlockRenderDetailsByIdQuery : IQuery<CustomEntityVersionPageBlockRenderDetails>
    {
        public GetCustomEntityVersionPageBlockRenderDetailsByIdQuery() { }

        /// <summary>
        /// Initializes the query with the specified parameters.
        /// </summary>
        /// <param name="customEntityVersionPageBlockId">Id of the custom entity page block version to find..</param>
        /// <param name="publishStatus">
        /// Optional publish status of the parent page or custom entity 
        /// being mapped. This is provided so related entities can use
        /// the same publish status when queried in the mapping process. 
        /// Defaults to PublishStatusQuery.Published.
        /// </param>
        public GetCustomEntityVersionPageBlockRenderDetailsByIdQuery(int customEntityVersionPageBlockId, PublishStatusQuery? publishStatus = null)
        {
            CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
            if (publishStatus.HasValue)
            {
                PublishStatus = publishStatus.Value;
            }
        }

        /// <summary>
        /// Id of the custom entity page block version to find.
        /// </summary>
        public int CustomEntityVersionPageBlockId { get; set; }

        /// <summary>
        /// Optional publish status of the parent page or custom entity 
        /// being mapped. This is provided so related entities can use
        /// the same publish status when queried in the mapping process. 
        /// Defaults to PublishStatusQuery.Published.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }
    }
}
