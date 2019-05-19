using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retieving custom entity page data for a set of custom 
    /// entity ids.
    /// </summary>
    public interface IAdvancedContentRepositoryPageByCustomEntityIdRangeQueryBuilder
    {
        /// <summary>
        /// Finds routing information for a set of custom entities by their ids. Although
        /// in a typical website you wouldn't have multiple details pages for a custom entity
        /// type, it is supported and so each custom entity id in the query returns a collection
        /// of routes.
        /// </summary>
        Task<IDictionary<int, ICollection<PageRoutingInfo>>> AsRoutingInfoAsync();
    }
}
