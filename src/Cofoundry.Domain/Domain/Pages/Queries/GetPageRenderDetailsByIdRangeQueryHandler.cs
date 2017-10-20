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
    public class GetPageRenderDetailsByIdRangeQueryHandler
        : IAsyncQueryHandler<GetPageRenderDetailsByIdRangeQuery, IDictionary<int, PageRenderDetails>>
        , IPermissionRestrictedQueryHandler<GetPageRenderDetailsByIdRangeQuery, IDictionary<int, PageRenderDetails>>
    {
        #region constructor

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

        #endregion

        #region execution

        public async Task<IDictionary<int, PageRenderDetails>> ExecuteAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbPages = await QueryPages(query).ToListAsync();
            var pages = dbPages
                .Select(_pageMapper.Map)
                .ToList();
                ;

            var pageRoutes = await _queryExecutor.GetByIdRangeAsync<PageRoute>(GetAllPageIds(pages), executionContext);
            MapPageRoutes(pages, pageRoutes);

            var dbPageBlocks = await QueryPageBlocks(pages).ToListAsync();
            var allBlockTypes = await _queryExecutor.GetAllAsync<PageBlockTypeSummary>(executionContext);

            await _entityVersionPageBlockMapper.MapRegionsAsync(dbPageBlocks, pages.SelectMany(p => p.Regions), allBlockTypes, query.WorkFlowStatus);

            return pages.ToDictionary(d => d.PageId);
        }
        
        private IQueryable<PageVersion> QueryPages(GetPageRenderDetailsByIdRangeQuery query)
        {
            if (query.WorkFlowStatus == WorkFlowStatusQuery.SpecificVersion)
            {
                throw new InvalidOperationException("WorkFlowStatusQuery.SpecificVersion not supported in GetPageRenderDetailsByIdRangeQuery");
            }

            IQueryable<PageVersion> dbQuery = _dbContext
                .PageVersions
                .AsNoTracking()
                .Include(v => v.Page)
                .Include(v => v.OpenGraphImageAsset)
                .Include(v => v.PageTemplate)
                .ThenInclude(t => t.PageTemplateRegions)
                .Where(v => query.PageIds.Contains(v.PageId) && !v.IsDeleted)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus);

            return dbQuery;
        }

        private IQueryable<PageVersionBlock> QueryPageBlocks(List<PageRenderDetails> pages)
        {
            var versionIds = pages.Select(p => p.PageVersionId);

            return _dbContext
                .PageVersionBlocks
                .AsNoTracking()
                .Where(m => versionIds.Contains(m.PageVersionId));
        }

        private IEnumerable<int> GetAllPageIds(List<PageRenderDetails> pages)
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

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRenderDetailsByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
