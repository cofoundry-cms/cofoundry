using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving page data for a unique database id.
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
        Task<PageRoute> AsRouteAsync();

        /// <summary>
        /// Gets a page PageRenderSummary projection by id, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published 
        /// versions only, but you can use the overload with the publishStatusQuery 
        /// parameter to control this behavior.
        /// </summary>
        Task<PageRenderSummary> AsRenderSummaryAsync();

        /// <summary>
        /// Gets a page PageRenderSummary projection by id, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. 
        /// The result is version-sensitive, use the publishStatusQuery parameter
        /// to control the version returned.
        /// </summary>
        /// <param name="publishStatusQuery">Used to determine which version of the page to include data for.</param>
        Task<PageRenderSummary> AsRenderSummaryAsync(PublishStatusQuery publishStatusQuery);

        /// <summary>
        /// Gets a projection of a page that contains the data required to render a 
        /// page, including template data for all the content-editable regions.
        /// The result is version-sensitive and defaults to returning published 
        /// versions only, but you can use the overload with the publishStatusQuery 
        /// parameter to control this behavior.
        /// </summary>
        Task<PageRenderDetails> AsRenderDetailsAsync();

        /// <summary>
        /// Gets a projection of a page that contains the data required to render a 
        /// page, including template data for all the content-editable regions.
        /// The result is version-sensitive, use the publishStatusQuery parameter
        /// to control the version returned.
        /// </summary>
        /// <param name="publishStatusQuery">Used to determine which version of the page to include data for.</param>
        Task<PageRenderDetails> AsRenderDetailsAsync(PublishStatusQuery publishStatusQuery);
    }
}
