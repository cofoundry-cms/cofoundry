using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retieving custom entity page data for a custom entity id.
    /// </summary>
    public interface IAdvancedContentRepositoryPageByCustomEntityIdQueryBuilder
    {
        /// <summary>
        /// Finds routing information for a custom entitiy by it's id. Although
        /// in a typical website you wouldn't have multiple details pages for a custom entity
        /// type, it is supported and the query returns a collection of routes.
        /// </summary>
        Task<ICollection<PageRoutingInfo>> AsRoutingInfoAsync();
    }
}
