using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving a set of pages using a batch of 
    /// database ids. The Cofoundry.Core dictionary extensions can be 
    /// useful for ordering the results e.g. results.FilterAndOrderByKeys(ids).
    /// </summary>
    public interface IContentRepositoryPageByIdRangeQueryBuilder
    {
        /// <summary>
        /// Query retuning page routing data for a set of pages by their database ids. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        IDomainRepositoryQueryContext<IDictionary<int, PageRoute>> AsRoutes();

        /// <summary>
        /// Query returning a range of pages by a set of id, projected as a PageRenderSummary, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The results are 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        IDomainRepositoryQueryContext<IDictionary<int, PageRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatus = null);

        /// <summary>
        /// Query returning a range of pages by their ids projected as PageRenderDetails models. A PageRenderDetails contains 
        /// the data required to render a page, including template data for all the content-editable regions.
        /// </summary>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        IDomainRepositoryQueryContext<IDictionary<int, PageRenderDetails>> AsRenderDetails(PublishStatusQuery? publishStatus = null);
    }
}
