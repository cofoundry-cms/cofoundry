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
    public interface IContentRepositoryCustomEntityByIdRangeQueryBuilder
    {
        /// <summary>
        /// Projects the result as a CustomEntityRenderSummary, which is a 
        /// general-purpose projection with version specific data, including 
        /// a deserialized data model. The results are version-sensitive and 
        /// defaults to returning published versions only, but this behavior 
        /// can be controlled by the publishStatus parameter.
        /// </summary>
        /// <param name="publishStatusQuery">Used to determine which version of the custom entities to include data for.</param>
        IDomainRepositoryQueryContext<IDictionary<int, CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatusQuery = null);
    }
}
