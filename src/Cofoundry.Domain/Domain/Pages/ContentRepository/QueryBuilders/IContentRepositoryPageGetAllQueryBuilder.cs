using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving page data for a unique database id.
    /// </summary>
    public interface IContentRepositoryPageGetAllQueryBuilder
    {
        /// <summary>
        /// Query returning a collection of page routing data for all pages. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<PageRoute>> AsRoutes();
    }
}
