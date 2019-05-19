using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retieving page data for a unique database id.
    /// </summary>
    public interface IContentRepositoryPageByIdQueryBuilder
    {
        /// <summary>
        /// Returns page routing data for a single page. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        Task<PageRoute> AsPageRouteAsync();

        /// <summary>
        /// Gets a page PageRenderSummary projection by id, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        Task<PageRenderSummary> AsPageRenderSummaryAsync(PublishStatusQuery? publishStatus = null);

        /// <summary>
        /// Gets a projection of a page that contains the data required to render a 
        /// page, including template data for all the content-editable regions.
        /// </summary>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        Task<PageRenderDetails> AsPageRenderDetailsAsync(PublishStatusQuery? publishStatus = null);
    }
}
