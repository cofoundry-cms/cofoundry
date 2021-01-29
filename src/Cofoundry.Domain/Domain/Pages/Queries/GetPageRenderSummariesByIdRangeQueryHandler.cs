using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to get a range of pages by a set of ids, projected as a PageRenderSummary, which is
    /// a lighter weight projection designed for rendering to a site when the 
    /// templates, region and block data is not required. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.
    /// </summary>
    public class GetPageRenderSummariesByIdRangeQueryHandler
        : IQueryHandler<GetPageRenderSummariesByIdRangeQuery, IDictionary<int, PageRenderSummary>>
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
                .Include(v => v.PageVersion)
                .ThenInclude(v => v.OpenGraphImageAsset)
                .AsNoTracking()
                .FilterActive()
                .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                .Where(v => query.PageIds.Contains(v.PageId))
                .Select(r => r.PageVersion)
                .ToListAsync();

            return dbResults;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRenderSummariesByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
