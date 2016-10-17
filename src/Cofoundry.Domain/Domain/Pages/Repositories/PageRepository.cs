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

        public bool DoesPageHaveDraftVersion(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new DoesPageHaveDraftVersionQuery(pageId), executionContext);
        }

        public IEnumerable<PageModuleTypeSummary> GetAllPageModuleTypeSummaries(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<PageModuleTypeSummary>(executionContext);
        }

        public Task<IEnumerable<PageModuleTypeSummary>> GetAllPageModuleTypeSummariesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);
        }

        public IEnumerable<PageRoute> GetAllPageRoutes(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<PageRoute>(executionContext);
        }

        public Task<IEnumerable<PageRoute>> GetAllPageRoutesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<PageRoute>(executionContext);
        }

        public Task<PageRoute> GetNotFoundPageRouteByPathAsync(GetNotFoundPageRouteByPathQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<PageDetails> GetPageDetailsByIdAsync(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageDetails>(id, executionContext);
        }

        public PageDetails GetPageDetailsById(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<PageDetails>(id, executionContext);
        }

        public PageModuleTypeSummary GetPageModuleTypeSummaryById(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<PageModuleTypeSummary>(id, executionContext);
        }

        public Task<PageRenderDetails> GetPageRenderDetailsAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public PageRoute GetPageRouteById(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<PageRoute>(pageId, executionContext);
        }

        public Task<PageRoute> GetPageRouteByIdAsync(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageRoute>(pageId, executionContext);
        }

        public IDictionary<int, PageRoute> GetPageRoutesByIdRange(int[] pageIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRange<PageRoute>(pageIds, executionContext);
        }

        public Task<IDictionary<int, PageRoute>> GetPageRoutesByIdRangeAsync(int[] pageIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRangeAsync<PageRoute>(pageIds, executionContext);
        }

        public IEnumerable<PageRoute> GetPageRoutesByWebDirectoryId(int webDirectoryId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetPageRoutesByWebDirectoryIdQuery(webDirectoryId), executionContext);
        }

        public IDictionary<int, IEnumerable<PageRoutingInfo>> GetPageRoutingInfoByCustomEntityIdRange(int[] customEntityIds, IExecutionContext executionContext = null)
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

        public Task<IDictionary<int, IEnumerable<PageRoutingInfo>>> GetPageRoutingInfoByCustomEntityIdRangeAsync(int[] customEntityIds, IExecutionContext executionContext = null)
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

        public IEnumerable<PageVersionDetails> GetPageVersionDetailsByPageId(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetPageVersionDetailsByPageIdQuery(pageId), executionContext);
        }

        public Task<IEnumerable<PageVersionDetails>> GetPageVersionDetailsByPageIdAsync(int pageId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetPageVersionDetailsByPageIdQuery(pageId), executionContext);
        }

        public PageVersionModuleRenderDetails GetPageVersionModuleRenderDetailsById(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query,  executionContext);
        }

        public Task<PageVersionModuleRenderDetails> GetPageVersionModuleRenderDetailsByIdAsync(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public bool IsPagePathUnique(IsPagePathUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<bool> IsPagePathUniqueAsync(IsPagePathUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<PagedQueryResult<PageSummary>> SearchPageSummariesAsync(SearchPageSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

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
