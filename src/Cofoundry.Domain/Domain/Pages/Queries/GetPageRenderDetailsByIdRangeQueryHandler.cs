using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a range of pages by their ids projected as PageRenderDetails models. A PageRenderDetails contains 
    /// the data required to render a page, including template data for all the content-editable regions.
    /// </summary>
    public class GetPageRenderDetailsByIdRangeQueryHandler
        : IQueryHandler<GetPageRenderDetailsByIdRangeQuery, IDictionary<int, PageRenderDetails>>
        , IPermissionRestrictedQueryHandler<GetPageRenderDetailsByIdRangeQuery, IDictionary<int, PageRenderDetails>>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageRenderDetailsMapper _pageMapper;
        private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;

        public GetPageRenderDetailsByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageRenderDetailsMapper pageMapper,
            IEntityVersionPageBlockMapper entityVersionPageBlockMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageMapper = pageMapper;
            _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
        }

        public async Task<IDictionary<int, PageRenderDetails>> ExecuteAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbPages = await GetPagesAsync(query, executionContext);

            var pageRoutesQuery = new GetPageRoutesByIdRangeQuery(GetAllPageIds(dbPages));
            var pageRoutes = await _queryExecutor.ExecuteAsync(pageRoutesQuery, executionContext);

            var pages = dbPages
                .Select(p => _pageMapper.Map(p, pageRoutes))
                .ToList();

            MapPageRoutes(pages, pageRoutes);

            var dbPageBlocks = await QueryPageBlocks(pages).ToListAsync();
            var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

            await _entityVersionPageBlockMapper.MapRegionsAsync(
                dbPageBlocks,
                pages.SelectMany(p => p.Regions),
                allBlockTypes,
                query.PublishStatus,
                executionContext
                );

            return pages.ToDictionary(d => d.PageId);
        }

        private async Task<IEnumerable<PageVersion>> GetPagesAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
        {
            if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
            {
                throw new InvalidOperationException($"{nameof(PublishStatusQuery)}.{nameof(PublishStatusQuery.SpecificVersion)} not supported in {nameof(GetPageRenderDetailsByIdRangeQuery)}");
            }

            var dbResults = await _dbContext
                .PagePublishStatusQueries
                .AsNoTracking()
                .Include(v => v.PageVersion)
                .ThenInclude(v => v.Page)
                .Include(v => v.PageVersion)
                .ThenInclude(v => v.OpenGraphImageAsset)
                .Include(v => v.PageVersion)
                .ThenInclude(v => v.PageTemplate)
                .ThenInclude(t => t.PageTemplateRegions)
                .FilterActive()
                .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                .Where(v => query.PageIds.Contains(v.PageId))
                .ToListAsync();

            return dbResults
                .Select(r => r.PageVersion);
        }

        private IQueryable<PageVersionBlock> QueryPageBlocks(List<PageRenderDetails> pages)
        {
            var versionIds = pages.Select(p => p.PageVersionId);

            return _dbContext
                .PageVersionBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => versionIds.Contains(m.PageVersionId));
        }

        private IEnumerable<int> GetAllPageIds(IEnumerable<PageVersion> pages)
        {
            return pages.Select(p => p.PageId);
        }

        private static void MapPageRoutes(List<PageRenderDetails> pages, IDictionary<int, PageRoute> pageRoutes)
        {
            foreach (var page in pages)
            {
                page.PageRoute = pageRoutes.GetOrDefault(page.PageId);
            }
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRenderDetailsByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }
    }
}
