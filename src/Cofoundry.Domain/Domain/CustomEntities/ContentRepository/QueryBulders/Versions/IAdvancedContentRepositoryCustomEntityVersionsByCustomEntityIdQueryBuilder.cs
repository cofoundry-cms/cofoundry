using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving paged collections of custom entity versions by a custom entity id.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder
    {
        /// <summary>
        /// Gets a set of custom entity versions for a specific 
        /// custom entity, ordered by create date, and returns
        /// them as a paged collection of CustomEntityVersionSummary
        /// projections.
        /// </summary>
        IDomainRepositoryQueryContext<PagedQueryResult<CustomEntityVersionSummary>> AsVersionSummaries(GetCustomEntityVersionSummariesByCustomEntityIdQuery query);
    }
}
