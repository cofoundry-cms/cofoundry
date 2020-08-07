using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving page blocks in custom entity pages by an id.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityPageBlockByIdQueryBuilder
    {
        /// <summary>
        /// Returns data for a specific block in a custom entity page version by 
        /// it's id. Because the mapped display model may contain other versioned 
        /// entities, you can optionally pass down a PublishStatusQuery to use 
        /// in the mapping process.
        /// </summary>
        IDomainRepositoryQueryContext<CustomEntityVersionPageBlockRenderDetails> AsRenderDetails(PublishStatusQuery? publishStatusQuery = null);
    }
}
