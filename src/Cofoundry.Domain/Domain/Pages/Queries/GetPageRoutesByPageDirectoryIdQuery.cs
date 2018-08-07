using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns page routing data for pages that are nested immediately inside the specified 
    /// directory. The PageRoute projection is a small page object focused on providing 
    /// routing data only. Data returned from this query is cached by 
    /// default as it's core to routing and often incorporated in more detailed
    /// page projections.
    /// </summary>
    public class GetPageRoutesByPageDirectoryIdQuery : IQuery<ICollection<PageRoute>>
    {
        /// <summary>
        /// Returns page routing data for pages that are nested immediately inside the specified 
        /// directory. The PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        public GetPageRoutesByPageDirectoryIdQuery() { }

        /// <summary>
        /// Returns page routing data for pages that are nested immediately inside the specified 
        /// directory. The PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        /// <param name="id">The id of the directory to get child pages for.</param>
        public GetPageRoutesByPageDirectoryIdQuery(int id)
        {
            PageDirectoryId = id;
        }

        /// <summary>
        /// The id of the directory to get child pages for.
        /// </summary>
        public int PageDirectoryId { get; set; }
    }
}
