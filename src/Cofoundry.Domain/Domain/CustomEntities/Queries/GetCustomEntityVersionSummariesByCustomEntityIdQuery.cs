using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a set of custom entity versions for a specific 
    /// custom entity, ordered by create date, and returns
    /// them as a paged collection of CustomEntityVersionSummary
    /// projections.
    /// </summary>
    public class GetCustomEntityVersionSummariesByCustomEntityIdQuery 
        : SimplePageableQuery
        , IQuery<PagedQueryResult<CustomEntityVersionSummary>>
    {
        /// <summary>
        /// Database id of the custom entity to filter on.
        /// </summary>
        public int CustomEntityId { get; set; }
    }
}
