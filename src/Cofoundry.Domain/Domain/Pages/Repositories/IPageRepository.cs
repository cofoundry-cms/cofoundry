using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over page data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    [Obsolete("Use the new IContentRepository instead.")]
    public interface IPageRepository
    {
        #region queries

        #region page routes

        /// <summary>
        /// Returns a collection of page routing data for all pages. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<ICollection<PageRoute>> GetAllPageRoutesAsync(IExecutionContext executionContext = null);

        /// <summary>
        /// Returns page routing data for a single page. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        /// <param name="pageId">Database id of the page to fetch routing data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageRoute> GetPageRouteByIdAsync(int pageId, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns page routing data for a set of pages by their database ids. The 
        /// PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        /// <param name="pageIds">Database ids of the pages to fetch routing data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<IDictionary<int, PageRoute>> GetPageRoutesByIdRangeAsync(IEnumerable<int> pageIds, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns page routing data for pages that are nested immediately inside the specified 
        /// directory. The PageRoute projection is a small page object focused on providing 
        /// routing data only. Data returned from this query is cached by 
        /// default as it's core to routing and often incorporated in more detailed
        /// page projections.
        /// </summary>
        /// <param name="pageDirectoryId">The id of the directory to get child pages for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<ICollection<PageRoute>> GetPageRoutesByPageDirectoryIdAsync(int pageDirectoryId, IExecutionContext executionContext = null);

        /// <summary>
        /// Attempts to find the most relevant 'Not Found' page route by searching
        /// for a 'Not Found' page up the directory tree.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageRoute> GetNotFoundPageRouteByPathAsync(GetNotFoundPageRouteByPathQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageRoutingInfo

        /// <summary>
        /// Finds routing information for a custom entitiy by it's id. Although
        /// in a typical website you wouldn't have multiple details pages for a custom entity
        /// type, it is supported and the query returns a collection of routes.
        /// </summary>
        /// <param name="customEntityId">Database id of the custom entity to find routing data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<ICollection<PageRoutingInfo>> GetPageRoutingInfoByCustomEntityIdAsync(int customEntityId, IExecutionContext executionContext = null);

        /// <summary>
        /// Finds routing information for a set of custom entities by their ids. Although
        /// in a typical website you wouldn't have multiple details pages for a custom entity
        /// type, it is supported and so each custom entity id in the query returns a collection
        /// of routes.
        /// </summary>
        /// <param name="customEntityIds">Database ids of the custom entities to find routing data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<IDictionary<int, ICollection<PageRoutingInfo>>> GetPageRoutingInfoByCustomEntityIdRangeAsync(IEnumerable<int> customEntityIds, IExecutionContext executionContext = null);

        /// <summary>
        /// Attempts to find a matching page route using the supplied path. The path
        /// has to be an absolute match, i.e. the query does not try and find a fall-back similar route.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageRoutingInfo> GetPageRoutingInfoByPathAsync(GetPageRoutingInfoByPathQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageRenderSummary

        /// <summary>
        /// Search page data returning the PageRenderSummary projection, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the PublishStatus query property.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PagedQueryResult<PageRenderSummary>> SearchPageRenderSummariesAsync(SearchPageRenderSummariesQuery query, IExecutionContext executionContext = null);

        /// <summary>
        /// Gets a page PageRenderSummary projection by id, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageRenderSummary> GetPageRenderDetailsByIdAsync(GetPageRenderSummaryByIdQuery query, IExecutionContext executionContext = null);

        /// <summary>
        /// Gets a range of pages by a set of id, projected as a PageRenderSummary, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The results are 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<IDictionary<int, PageRenderSummary>> GetPageRenderSummariesByIdRangeAsync(GetPageRenderSummariesByIdRangeQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageRenderDetails

        /// <summary>
        /// Gets a projection of a page that contains the data required to render a page, including template 
        /// data for all the content-editable regions.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageRenderDetails> GetPageRenderDetailsByIdAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext = null);
        
        /// <summary>
        /// Gets a range of pages by their ids projected as PageRenderDetails models. A PageRenderDetails contains 
        /// the data required to render a page, including template data for all the content-editable regions.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<IDictionary<int, PageRenderDetails>> GetPageRenderDetailsByIdRangeAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageRegionDetails

        /// <summary>
        /// Returns a collection of the content managed regions and
        /// blocks for a specific version of a page.
        /// </summary>
        /// <param name="pageVersionId">Database id of the page version to get content data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<ICollection<PageRegionDetails>> GetPageRegionDetailsByPageVersionIdAsync(int pageVersionId, IExecutionContext executionContext = null);

        #endregion

        #region PageVersionBlockRenderDetails

        /// <summary>
        /// Returns data for a specific block in a page version by it's id. Because
        /// the mapped display model may contain other versioned entities, you can 
        /// optionally pass down a PublishStatusQuery to use in the mapping process.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageVersionBlockRenderDetails> GetPageVersionBlockRenderDetailsByIdAsync(GetPageVersionBlockRenderDetailsByIdQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageSummary (admin)
        
        /// <summary>
        /// Finds pages with the specified page ids and returns them as PageSummary 
        /// objects. Note that this query does not account for WorkFlowStatus and so
        /// pages will be returned irrecpective of whether they aree published or not.
        /// </summary>
        /// <param name="pageIds">A collection of database ids of the pages to fetch.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<IDictionary<int, PageSummary>> GetPageSummariesByIdRangeAsync(IEnumerable<int> pageIds, IExecutionContext executionContext = null);

        /// <summary>
        /// Search page data returning the PageSummary projection, which is primarily used
        /// to display lists of page information in the admin panel. The query isn't version 
        /// specific and should not be used to render content out to a live page because some of
        /// the pages returned may be unpublished.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PagedQueryResult<PageSummary>> SearchPageSummariesAsync(SearchPageSummariesQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageDetails (admin)

        /// <summary>
        /// Returns detailed information on a page and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the PageDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        /// <param name="pageId">Database id of the page to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageDetails> GetPageDetailsByIdAsync(int pageId, IExecutionContext executionContext = null);

        #endregion

        #region PageVersionSummary (admin)

        /// <summary>
        /// Returns a paged collection of versions of a specific page, ordered 
        /// historically with the latest/draft version first.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PagedQueryResult<PageVersionSummary>> GetPageVersionSummariesByPageIdAsync(GetPageVersionSummariesByPageIdQuery query, IExecutionContext executionContext = null);

        #endregion

        #region utility

        /// <summary>
        /// Determines if a page has a draft version of not. A page can only have one draft
        /// version at a time.
        /// </summary>
        /// <param name="pageId">Id of the page to check.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<bool> DoesPageHaveDraftVersionAsync(int pageId, IExecutionContext executionContext = null);

        /// <summary>
        /// Determines if a page path already exists. Page paths are made
        /// up of a locale, directory and url slug; duplicates are not permitted.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<bool> IsPagePathUniqueAsync(IsPagePathUniqueQuery query, IExecutionContext executionContext = null);

        #endregion

        #endregion

        #region commands

        Task<int> AddPageAsync(AddPageCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Creates a new draft version of a page from the currently published version. If there
        /// isn't a currently published version then an exception will be thrown. An exception is also 
        /// thrown if there is already a draft version.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        Task<int> AddPageDraftVersionAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext = null);

        Task<int> AddPageVersionBlockAsync(AddPageVersionBlockCommand command, IExecutionContext executionContext = null);

        Task DeletePageAsync(int pageId, IExecutionContext executionContext = null);

        Task DeletePageDraftVersionAsync(int pageId, IExecutionContext executionContext = null);

        Task DeletePageVersionBlockAsync(int pageVersionBlockId, IExecutionContext executionContext = null);

        Task DuplicatePageAsync(DuplicatePageCommand command, IExecutionContext executionContext = null);

        Task MovePageVersionBlockAsync(MovePageVersionBlockCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Publishes a page. If the page is already published and
        /// a date is specified then the publish date will be updated.
        /// </summary>
        /// <param name="command">Publishing command to execute.</param>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        Task PublishPageCommandAsync(PublishPageCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Sets the status of a page to un-published, but does not
        /// remove the publish date, which is preserved so that it
        /// can be used as a default when the user chooses to publish
        /// again.
        /// </summary>
        /// <param name="pageId">The id of the page to set un-published.</param>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        Task UnPublishPageCommandAsync(int pageId, IExecutionContext executionContext = null);

        Task UpdatePageAsync(UpdatePageCommand command, IExecutionContext executionContext = null);

        Task UpdatePageDraftVersionAsync(UpdatePageDraftVersionCommand command, IExecutionContext executionContext = null);

        Task UpdatePageUrlAsync(UpdatePageUrlCommand command, IExecutionContext executionContext = null);

        Task UpdatePageVersionBlockAsync(UpdatePageVersionBlockCommand command, IExecutionContext executionContext = null);

        #endregion
    }
}
