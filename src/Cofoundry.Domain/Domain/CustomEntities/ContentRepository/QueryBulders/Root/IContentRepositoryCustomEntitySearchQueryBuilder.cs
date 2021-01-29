using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to search for custom entity data, returning paged lists of data.
    /// </summary>
    public interface IContentRepositoryCustomEntitySearchQueryBuilder
    {
        /// <summary>
        /// <para>
        /// Search for custom entities of a specific type and return them
        /// as a render summary projection which is a general-purpose projection 
        /// of a custom entity with version specific data, including a deserialized 
        /// data model.
        /// </para>
        /// <para>
        /// The query is version-sensitive and defaults to returning published 
        /// versions only, but this behavior can be controlled by the PublishStatus 
        /// query property.
        /// </para>
        /// </summary>
        /// <param name="query">Criteria to filter results by.</param>
        /// <returns>Paged set of results.</returns>
        IDomainRepositoryQueryContext<PagedQueryResult<CustomEntityRenderSummary>> AsRenderSummaries(SearchCustomEntityRenderSummariesQuery query);
    }
}
