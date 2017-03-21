using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over page data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    public class PageRepository : IPageRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public PageRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        #region page routes

        public IEnumerable<PageRoute> GetAllPageRoutes(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<PageRoute>(executionContext);
        }

        public Task<IEnumerable<PageRoute>> GetAllPageRoutesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<PageRoute>(executionContext);
        }
        public PageRoute GetPageRouteById(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<PageRoute>(pageId, executionContext);
        }

        public Task<PageRoute> GetPageRouteByIdAsync(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageRoute>(pageId, executionContext);
        }

        public IDictionary<int, PageRoute> GetPageRoutesByIdRange(IEnumerable<int> pageIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRange<PageRoute>(pageIds, executionContext);
        }

        public Task<IDictionary<int, PageRoute>> GetPageRoutesByIdRangeAsync(IEnumerable<int> pageIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRangeAsync<PageRoute>(pageIds, executionContext);
        }

        public IEnumerable<PageRoute> GetPageRoutesByWebDirectoryId(int webDirectoryId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetPageRoutesByWebDirectoryIdQuery(webDirectoryId), executionContext);
        }

        public Task<PageRoute> GetNotFoundPageRouteByPathAsync(GetNotFoundPageRouteByPathQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region PageRoutingInfo

        public IDictionary<int, IEnumerable<PageRoutingInfo>> GetPageRoutingInfoByCustomEntityIdRange(IEnumerable<int> customEntityIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetPageRoutingInfoByCustomEntityIdRangeQuery(customEntityIds), executionContext);
        }

        public IEnumerable<PageRoutingInfo> GetPageRoutingInfoByCustomEntityId(int customEntityId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetPageRoutingInfoByCustomEntityIdQuery(customEntityId), executionContext);
        }

        public Task<IEnumerable<PageRoutingInfo>> GetPageRoutingInfoByCustomEntityIdAsync(int customEntityId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetPageRoutingInfoByCustomEntityIdQuery(customEntityId), executionContext);
        }

        public Task<IDictionary<int, IEnumerable<PageRoutingInfo>>> GetPageRoutingInfoByCustomEntityIdRangeAsync(IEnumerable<int> customEntityIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetPageRoutingInfoByCustomEntityIdRangeQuery(customEntityIds), executionContext);
        }

        /// <summary>
        /// Attempts to find a matching page route using the supplied path. The path
        /// has to be an absolute match, i.e. the query does not try and find a fall-back similar route.
        /// </summary>
        public Task<PageRoutingInfo> GetPageRoutingInfoByPathAsync(GetPageRoutingInfoByPathQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region PageRenderDetails

        /// <summary>
        /// Gets a page object that contains the data required to render a page, including template 
        /// data for all the content-editable sections.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public PageRenderDetails GetPageRenderDetails(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        /// <summary>
        /// Gets a page object that contains the data required to render a page, including template 
        /// data for all the content-editable sections.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<PageRenderDetails> GetPageRenderDetailsAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Gets a range of pages by their PageIds as PageRenderDetails objects. A PageRenderDetails contains 
        /// the data required to render a page, including template data for all the content-editable sections.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public IDictionary<int, PageRenderDetails> GetPageRenderDetailsByIdRange(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        /// <summary>
        /// Gets a range of pages by their PageIds as PageRenderDetails objects. A PageRenderDetails contains 
        /// the data required to render a page, including template data for all the content-editable sections.
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<IDictionary<int, PageRenderDetails>> GetPageRenderDetailsByIdRangeAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region PageSectionDetails

        /// <summary>
        /// Gets a collection of the content managed sections and
        /// modules for a specific version of a page. These are the 
        /// modules that get rendered in the page template linked
        /// to the page version.
        /// </summary>
        /// <param name="pageVersionId">Database id of the page version to get content data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public IEnumerable<PageSectionDetails> GetPageSectionDetailsByPageVersionId(int pageVersionId, IExecutionContext executionContext = null)
        {
            var query = new GetPageSectionDetailsByPageVersionIdQuery(pageVersionId);
            return _queryExecutor.Execute(query, executionContext);
        }

        /// <summary>
        /// Gets a collection of the content managed sections and
        /// modules for a specific version of a page. These are the 
        /// modules that get rendered in the page template linked
        /// to the page version.
        /// </summary>
        /// <param name="pageVersionId">Database id of the page version to get content data for.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<IEnumerable<PageSectionDetails>> GetPageSectionDetailsByPageVersionIdAsync(int pageVersionId, IExecutionContext executionContext = null)
        {
            var query = new GetPageSectionDetailsByPageVersionIdQuery(pageVersionId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region PageVersionModuleRenderDetails

        public PageVersionModuleRenderDetails GetPageVersionModuleRenderDetailsById(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query,  executionContext);
        }

        public Task<PageVersionModuleRenderDetails> GetPageVersionModuleRenderDetailsByIdAsync(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region PageSummary (admin)

        /// <summary>
        /// Finds pages with the specified page ids and returns them as PageSummary 
        /// objects. Note that this query does not account for WorkFlowStatus and so
        /// pages will be returned irrecpective of whether they are published or not.
        /// </summary>
        /// <param name="pageIds">A collection of database ids of the pages to fetch.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public IDictionary<int, PageSummary> GetPageSummariesByPageId(IEnumerable<int> pageIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetPageSummariesByIdRangeQuery(pageIds), executionContext);
        }

        /// <summary>
        /// Finds pages with the specified page ids and returns them as PageSummary 
        /// objects. Note that this query does not account for WorkFlowStatus and so
        /// pages will be returned irrecpective of whether they aree published or not.
        /// </summary>
        /// <param name="pageIds">A collection of database ids of the pages to fetch.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<IDictionary<int, PageSummary>> GetPageSummariesByPageIdAsync(IEnumerable<int> pageIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetPageSummariesByIdRangeQuery(pageIds), executionContext);
        }

        public Task<PagedQueryResult<PageSummary>> SearchPageSummariesAsync(SearchPageSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region PageDetails (admin)

        public Task<PageDetails> GetPageDetailsByIdAsync(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageDetails>(id, executionContext);
        }

        public PageDetails GetPageDetailsById(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<PageDetails>(id, executionContext);
        }

        #endregion

        #region PageVersionSummary (admin)

        public IEnumerable<PageVersionSummary> GetPageVersionSummariesByPageId(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetPageVersionSummariesByPageIdQuery(pageId), executionContext);
        }

        public Task<IEnumerable<PageVersionSummary>> GetPageVersionSummariesByPageIdAsync(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetPageVersionSummariesByPageIdQuery(pageId), executionContext);
        }

        #endregion

        #region page module types

        public IEnumerable<PageModuleTypeSummary> GetAllPageModuleTypeSummaries(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<PageModuleTypeSummary>(executionContext);
        }

        public Task<IEnumerable<PageModuleTypeSummary>> GetAllPageModuleTypeSummariesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);
        }

        public PageModuleTypeSummary GetPageModuleTypeSummaryById(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<PageModuleTypeSummary>(id, executionContext);
        }

        #endregion

        #region utility

        public bool DoesPageHaveDraftVersion(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new DoesPageHaveDraftVersionQuery(pageId), executionContext);
        }

        public bool IsPagePathUnique(IsPagePathUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<bool> IsPagePathUniqueAsync(IsPagePathUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #endregion

        #region commands

        public async Task<int> AddPageAsync(AddPageCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputPageId;
        }

        public async Task<int> AddPageDraftVersionAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputPageVersionId;
        }

        public async Task<int> AddPageVersionModuleAsync(AddPageVersionModuleCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputPageModuleId;
        }

        public Task DeletePageAsync(int pageId, IExecutionContext executionContext = null)
        {
            var command = new DeletePageCommand() { PageId = pageId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task DeletePageDraftVersionAsync(int pageId, IExecutionContext executionContext = null)
        {
            var command = new DeletePageDraftVersionCommand() { PageId = pageId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task DeletePageVersionModuleAsync(int pageVersionModuleId, IExecutionContext executionContext = null)
        {
            var command = new DeletePageVersionModuleCommand() { PageVersionModuleId = pageVersionModuleId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task DuplicatePageAsync(DuplicatePageCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task MovePageVersionModuleAsync(MovePageVersionModuleCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task PublishPageCommandAsync(int pageId, IExecutionContext executionContext = null)
        {
            var command = new PublishPageCommand(pageId);
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UnPublishPageCommandAsync(int pageId, IExecutionContext executionContext = null)
        {
            var command = new UnPublishPageCommand() { PageId = pageId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdatePageAsync(UpdatePageCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdatePageDraftVersionAsync(UpdatePageDraftVersionCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdatePageUrlAsync(UpdatePageUrlCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdatePageVersionModuleAsync(UpdatePageVersionModuleCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        #endregion
    }
}
