using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a range of pages by their PageIds as PageRenderDetails objects. A PageRenderDetails contains 
    /// the data required to render a page, including template data for all the content-editable sections.
    /// </summary>
    public class GetPageRenderDetailsByIdRangeQueryHandler
        : IAsyncQueryHandler<GetPageRenderDetailsByIdRangeQuery, IDictionary<int, PageRenderDetails>>
        , IPermissionRestrictedQueryHandler<GetPageRenderDetailsByIdRangeQuery, IDictionary<int, PageRenderDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageMapper _pageMapper;
        private readonly IEntityVersionPageModuleMapper _entityVersionPageModuleMapper;

        public GetPageRenderDetailsByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageMapper pageMapper,
            IEntityVersionPageModuleMapper entityVersionPageModuleMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageMapper = pageMapper;
            _entityVersionPageModuleMapper = entityVersionPageModuleMapper;
        }

        #endregion

        #region public methods

        public async Task<IDictionary<int, PageRenderDetails>> ExecuteAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbPages = await QueryPages(query).ToListAsync();
            var pages = dbPages
                .Select(_pageMapper.MapRenderDetails)
                .ToList();
                ;

            var pageRoutes = await _queryExecutor.GetByIdRangeAsync<PageRoute>(GetAllPageIds(pages), executionContext);
            MapPageRoutes(pages, pageRoutes);

            var dbModules = await QueryModules(pages).ToListAsync();
            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);

            await _entityVersionPageModuleMapper.MapSectionsAsync(dbModules, pages.SelectMany(p => p.Sections), allModuleTypes, query.WorkFlowStatus);

            return pages.ToDictionary(d => d.PageId);
        }

        #endregion

        #region private helpers

        private IQueryable<PageVersion> QueryPages(GetPageRenderDetailsByIdRangeQuery query)
        {
            if (query.WorkFlowStatus == WorkFlowStatusQuery.SpecificVersion)
            {
                throw new InvalidOperationException("WorkFlowStatusQuery.SpecificVersion not supported in GetPageRenderDetailsByIdRangeQuery");
            }

            IQueryable<PageVersion> dbQuery = _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(v => query.PageIds.Contains(v.PageId) && !v.IsDeleted)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus)
                .Include(v => v.Page)
                .Include(v => v.PageTemplate)
                .Include(v => v.PageTemplate.PageTemplateSections);

            return dbQuery;
        }

        private IQueryable<PageVersionModule> QueryModules(List<PageRenderDetails> pages)
        {
            var versionIds = pages.Select(p => p.PageVersionId);

            return _dbContext
                .PageVersionModules
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
