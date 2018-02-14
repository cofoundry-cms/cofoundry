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
    public interface IPageRepository
    {
        #region queries

        #region page routes

        Task<ICollection<PageRoute>> GetAllPageRoutesAsync(IExecutionContext executionContext = null);

        Task<PageRoute> GetPageRouteByIdAsync(int pageId, IExecutionContext executionContext = null);

        Task<IDictionary<int, PageRoute>> GetPageRoutesByIdRangeAsync(IEnumerable<int> pageIds, IExecutionContext executionContext = null);

        Task<ICollection<PageRoute>> GetPageRoutesByPageDirectoryIdAsync(int pageDirectoryId, IExecutionContext executionContext = null);

        Task<PageRoute> GetNotFoundPageRouteByPathAsync(GetNotFoundPageRouteByPathQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageRoutingInfo
        
        Task<ICollection<PageRoutingInfo>> GetPageRoutingInfoByCustomEntityIdAsync(int customEntityId, IExecutionContext executionContext = null);

        Task<IDictionary<int, ICollection<PageRoutingInfo>>> GetPageRoutingInfoByCustomEntityIdRangeAsync(IEnumerable<int> customEntityIds, IExecutionContext executionContext = null);

        /// <summary>
        /// Attempts to find a matching page route using the supplied path. The path
        /// has to be an absolute match, i.e. the query does not try and find a fall-back similar route.
        /// </summary>
        Task<PageRoutingInfo> GetPageRoutingInfoByPathAsync(GetPageRoutingInfoByPathQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageRenderDetails
        
        /// <summary>
        /// Gets a page object that contains the data required to render a page, including template 
        /// data for all the content-editable regions.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageRenderDetails> GetPageRenderDetailsByIdAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext = null);
        
        /// <summary>
        /// Gets a range of pages by their PageIds as PageRenderDetails objects. A PageRenderDetails contains 
        /// the data required to render a page, including template data for all the content-editable regions.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<IDictionary<int, PageRenderDetails>> GetPageRenderDetailsByIdRangeAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageRegionDetails
        
        /// <summary>
        /// Gets a collection of the content managed regions and
        /// blocks for a specific version of a page. These are the 
        /// content blocks that get rendered in the page template linked
        /// to the page version.
        /// </summary>
        /// <param name="pageVersionId">Database id of the page version to get content data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<ICollection<PageRegionDetails>> GetPageRegionDetailsByPageVersionIdAsync(int pageVersionId, IExecutionContext executionContext = null);

        #endregion

        #region PageVersionBlockRenderDetails

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
        Task<IDictionary<int, PageSummary>> GetPageSummariesByPageIdAsync(IEnumerable<int> pageIds, IExecutionContext executionContext = null);
        
        Task<PagedQueryResult<PageSummary>> SearchPageSummariesAsync(SearchPageSummariesQuery query, IExecutionContext executionContext = null);

        #endregion

        #region PageDetails (admin)

        Task<PageDetails> GetPageDetailsByIdAsync(int pageId, IExecutionContext executionContext = null);

        #endregion

        #region PageVersionSummary (admin)
        
        Task<ICollection<PageVersionSummary>> GetPageVersionSummariesByPageIdAsync(int pageId, IExecutionContext executionContext = null);

        #endregion

        #region utility

        Task<bool> DoesPageHaveDraftVersionAsync(int pageId, IExecutionContext executionContext = null);

        Task<bool> IsPagePathUniqueAsync(IsPagePathUniqueQuery query, IExecutionContext executionContext = null);

        #endregion

        #endregion

        #region commands

        Task<int> AddPageAsync(AddPageCommand command, IExecutionContext executionContext = null);

        Task<int> AddPageDraftVersionAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext = null);

        Task<int> AddPageVersionBlockAsync(AddPageVersionBlockCommand command, IExecutionContext executionContext = null);

        Task DeletePageAsync(int pageId, IExecutionContext executionContext = null);

        Task DeletePageDraftVersionAsync(int pageId, IExecutionContext executionContext = null);

        Task DeletePageVersionBlockAsync(int pageVersionBlockId, IExecutionContext executionContext = null);

        Task DuplicatePageAsync(DuplicatePageCommand command, IExecutionContext executionContext = null);

        Task MovePageVersionBlockAsync(MovePageVersionBlockCommand command, IExecutionContext executionContext = null);

        Task PublishPageCommandAsync(PublishPageCommand command, IExecutionContext executionContext = null);

        Task UnPublishPageCommandAsync(int pageId, IExecutionContext executionContext = null);

        Task UpdatePageAsync(UpdatePageCommand command, IExecutionContext executionContext = null);

        Task UpdatePageDraftVersionAsync(UpdatePageDraftVersionCommand command, IExecutionContext executionContext = null);

        Task UpdatePageUrlAsync(UpdatePageUrlCommand command, IExecutionContext executionContext = null);

        Task UpdatePageVersionBlockAsync(UpdatePageVersionBlockCommand command, IExecutionContext executionContext = null);

        #endregion
    }
}
