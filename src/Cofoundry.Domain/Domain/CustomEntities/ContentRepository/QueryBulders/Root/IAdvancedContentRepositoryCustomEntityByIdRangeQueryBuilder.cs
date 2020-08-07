using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving a set of custom entities using a batch of 
    /// ids. The Cofoundry.Core dictionary extensions can be 
    /// useful for ordering the results e.g. results.FilterAndOrderByKeys(ids).
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityByIdRangeQueryBuilder
        : IContentRepositoryCustomEntityByIdRangeQueryBuilder
    {
        /// <summary>
        /// Projects the result as a CustomEntitySummary, which includes basic
        /// custom entity information with publish status and model data for the
        /// latest version. The query is not version-sensitive and is designed to be 
        /// used in the admin panel and not in a version-sensitive context such as a 
        /// public webpage.
        /// </summary>
        IDomainRepositoryQueryContext<IDictionary<int, CustomEntitySummary>> AsSummaries();
    }
}
