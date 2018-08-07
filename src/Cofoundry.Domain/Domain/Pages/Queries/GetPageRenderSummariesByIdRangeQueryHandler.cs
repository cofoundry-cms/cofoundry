using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a range of pages by their PageIds as PageRenderDetails objects. A PageRenderDetails contains 
    /// the data required to render a page, including template data for all the content-editable regions.
    /// </summary>
    public class GetPageRenderSummariesByIdRangeQueryHandler
        : IAsyncQueryHandler<GetPageRenderSummariesByIdRangeQuery, IDictionary<int, PageRenderSummary>>
        , IPermissionRestrictedQueryHandler<GetPageRenderSummariesByIdRangeQuery, IDictionary<int, PageRenderSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageRenderSummaryMapper _pageRenderSummaryMapper;

        public GetPageRenderSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageRenderSummaryMapper pageRenderSummaryMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageRenderSummaryMapper = pageRenderSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, PageRenderSummary>> ExecuteAsync(GetPageRenderSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbPages = await GetPagesAsync(query, executionContext);

            var allPageIds = dbPages.Select(p => p.PageId);
            var pageRoutesQuery = new GetPageRoutesByIdRangeQuery(allPageIds);
            var pageRoutes = await _queryExecutor.ExecuteAsync(pageRoutesQuery, executionContext);

            var pages = dbPages
                .Select(p => _pageRenderSummaryMapper.Map<PageRenderSummary>(p, pageRoutes))
                .ToList();

            return pages.ToDictionary(d => d.PageId);
        }
        
        private async Task<List<PageVersion>> GetPagesAsync(GetPageRenderSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
            {
                throw new InvalidOperationException($"PublishStatusQuery.SpecificVersion not supported in ${nameof(GetPageRenderSummariesByIdRangeQuery)}");
            }

            var dbResults = await _dbContext
                .PagePublishStatusQueries
                .AsNoTracking()
                .FilterActive()
                .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                .Where(v => query.PageIds.Contains(v.PageId))
                .Select(r => r.PageVersion)
                .Include(v => v.OpenGraphImageAsset)
                .ToListAsync();

            return dbResults;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRenderSummariesByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
