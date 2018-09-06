using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns page routing data for a set of pages by their database ids. The 
    /// PageRoute projection is a small page object focused on providing 
    /// routing data only. Data returned from this query is cached by 
    /// default as it's core to routing and often incorporated in more detailed
    /// page projections.
    /// </summary>
    public class GetPageRoutesByIdRangeQuery : IQuery<IDictionary<int, PageRoute>>
    {
        /// <summary>
        /// Returns page routing data for a set of pages by their database ids. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        public GetPageRoutesByIdRangeQuery() { }

        /// <summary>
        /// Returns page routing data for a set of pages by their database ids. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        /// <param name="pageIds">Database id of the page to fetch routing data for.</param>
        public GetPageRoutesByIdRangeQuery(IEnumerable<int> pageIds)
            : this(pageIds?.ToList())
        {
        }

        /// <summary>
        /// Returns page routing data for a set of pages by their database ids. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        /// <param name="pageIds">Database id of the page to fetch routing data for.</param>
        public GetPageRoutesByIdRangeQuery(IReadOnlyCollection<int> pageIds)
        {
            PageIds = pageIds;
        }

        /// <summary>
        /// Database id of the page to fetch routing data for.
        /// </summary>
        public IReadOnlyCollection<int> PageIds { get; set; }
    }
}
