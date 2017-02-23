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
        Task<int> AddPageAsync(AddPageCommand command, IExecutionContext executionContext = null);
        Task<int> AddPageDraftVersionAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext = null);
        Task<int> AddPageVersionModuleAsync(AddPageVersionModuleCommand command, IExecutionContext executionContext = null);
        Task DeletePageAsync(int pageId, IExecutionContext executionContext = null);
        Task DeletePageDraftVersionAsync(int pageId, IExecutionContext executionContext = null);
        Task DeletePageVersionModuleAsync(int pageVersionModuleId, IExecutionContext executionContext = null);
        bool DoesPageHaveDraftVersion(int pageId, IExecutionContext executionContext = null);
        Task DuplicatePageAsync(DuplicatePageCommand command, IExecutionContext executionContext = null);
        IEnumerable<PageModuleTypeSummary> GetAllPageModuleTypeSummaries(IExecutionContext executionContext = null);
        Task<IEnumerable<PageModuleTypeSummary>> GetAllPageModuleTypeSummariesAsync(IExecutionContext executionContext = null);
        IEnumerable<PageRoute> GetAllPageRoutes(IExecutionContext executionContext = null);
        Task<IEnumerable<PageRoute>> GetAllPageRoutesAsync(IExecutionContext executionContext = null);
        Task<PageRoute> GetNotFoundPageRouteByPathAsync(GetNotFoundPageRouteByPathQuery query, IExecutionContext executionContext = null);

        PageDetails GetPageDetailsById(int id, IExecutionContext executionContext = null);
        Task<PageDetails> GetPageDetailsByIdAsync(int id, IExecutionContext executionContext = null);

        IEnumerable<PageSectionDetails> GetPageSectionDetailsByPageVersionId(int pageVersionId, IExecutionContext executionContext = null);
        Task<IEnumerable<PageSectionDetails>> GetPageSectionDetailsByPageVersionIdAsync(int pageVersionId, IExecutionContext executionContext = null);
        
        PageModuleTypeSummary GetPageModuleTypeSummaryById(int id, IExecutionContext executionContext = null);

        Task<PageRenderDetails> GetPageRenderDetailsAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext = null);

        PageRoute GetPageRouteById(int pageId, IExecutionContext executionContext = null);
        Task<PageRoute> GetPageRouteByIdAsync(int pageId, IExecutionContext executionContext = null);

        /// <summary>
        /// Attempts to find a matching page route using the supplied path. The path
        /// has to be an absolute match, i.e. the query does not try and find a fall-back similar route.
        /// </summary>
        Task<PageRoutingInfo> GetPageRoutingInfoByPathAsync(GetPageRoutingInfoByPathQuery query, IExecutionContext executionContext = null);
        IDictionary<int, PageRoute> GetPageRoutesByIdRange(int[] pageIds, IExecutionContext executionContext = null);
        Task<IDictionary<int, PageRoute>> GetPageRoutesByIdRangeAsync(int[] pageIds, IExecutionContext executionContext = null);
        IEnumerable<PageRoute> GetPageRoutesByWebDirectoryId(int webDirectoryId, IExecutionContext executionContext = null);
        IEnumerable<PageRoutingInfo> GetPageRoutingInfoByCustomEntityId(int customEntityId, IExecutionContext executionContext = null);
        Task<IEnumerable<PageRoutingInfo>> GetPageRoutingInfoByCustomEntityIdAsync(int customEntityId, IExecutionContext executionContext = null);
        IDictionary<int, IEnumerable<PageRoutingInfo>> GetPageRoutingInfoByCustomEntityIdRange(int[] customEntityIds, IExecutionContext executionContext = null);
        Task<IDictionary<int, IEnumerable<PageRoutingInfo>>> GetPageRoutingInfoByCustomEntityIdRangeAsync(int[] customEntityIds, IExecutionContext executionContext = null);
        Task<IEnumerable<PageVersionSummary>> GetPageVersionSummariesByPageIdAsync(int pageId, IExecutionContext executionContext = null);
        IEnumerable<PageVersionSummary> GetPageVersionSummariesByPageId(int pageId, IExecutionContext executionContext = null);
        Task<PageVersionModuleRenderDetails> GetPageVersionModuleRenderDetailsByIdAsync(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext = null);
        PageVersionModuleRenderDetails GetPageVersionModuleRenderDetailsById(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext = null);
        bool IsPagePathUnique(IsPagePathUniqueQuery query, IExecutionContext executionContext = null);
        Task<bool> IsPagePathUniqueAsync(IsPagePathUniqueQuery query, IExecutionContext executionContext = null);
        Task MovePageVersionModuleAsync(MovePageVersionModuleCommand command, IExecutionContext executionContext = null);
        Task PublishPageCommandAsync(int pageId, IExecutionContext executionContext = null);
        Task<PagedQueryResult<PageSummary>> SearchPageSummariesAsync(SearchPageSummariesQuery query, IExecutionContext executionContext = null);
        Task UnPublishPageCommandAsync(int pageId, IExecutionContext executionContext = null);
        Task UpdatePageAsync(UpdatePageCommand command, IExecutionContext executionContext = null);
        Task UpdatePageDraftVersionAsync(UpdatePageDraftVersionCommand command, IExecutionContext executionContext = null);
        Task UpdatePageUrlAsync(UpdatePageUrlCommand command, IExecutionContext executionContext = null);
        Task UpdatePageVersionModuleAsync(UpdatePageVersionModuleCommand command, IExecutionContext executionContext = null);
    }
}
