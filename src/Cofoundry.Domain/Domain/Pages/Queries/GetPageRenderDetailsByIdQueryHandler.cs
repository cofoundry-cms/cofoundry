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
    /// Gets a projection of a page that contains the data required to render a page, including template 
    /// data for all the content-editable regions.
    /// </summary>
    public class GetPageRenderDetailsByIdQueryHandler 
        : IQueryHandler<GetPageRenderDetailsByIdQuery, PageRenderDetails>
        , IPermissionRestrictedQueryHandler<GetPageRenderDetailsByIdQuery, PageRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageRenderDetailsMapper _pageRenderDetailsMapper;
        private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;

        public GetPageRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageRenderDetailsMapper pageMapper,
            IEntityVersionPageBlockMapper entityVersionPageBlockMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageRenderDetailsMapper = pageMapper;
            _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
        }

        #endregion

        #region execution

        public async Task<PageRenderDetails> ExecuteAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbPage = await QueryPageAsync(query, executionContext);
            if (dbPage == null) return null;

            var pageRouteQuery = new GetPageRouteByIdQuery(dbPage.PageId);
            var pageRoute = await _queryExecutor.ExecuteAsync(pageRouteQuery, executionContext);

            var page = _pageRenderDetailsMapper.Map(dbPage, pageRoute);

            var dbPageBlocks = await QueryPageBlocks(page).ToListAsync();
            var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

            await _entityVersionPageBlockMapper.MapRegionsAsync(dbPageBlocks, page.Regions, allBlockTypes, query.PublishStatus, executionContext);

            return page;
        }

        private async Task<PageVersion> QueryPageAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            PageVersion result;

            if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
            {
                if (!query.PageVersionId.HasValue)
                {
                    throw new Exception("A PageVersionId must be included in the query to use PublishStatusQuery.SpecificVersion");
                }

                result = await _dbContext
                    .PageVersions
                    .AsNoTracking()
                    .Include(v => v.Page)
                    .Include(v => v.OpenGraphImageAsset)
                    .Include(v => v.PageTemplate)
                    .ThenInclude(t => t.PageTemplateRegions)
                    .FilterActive()
                    .FilterByPageId(query.PageId)
                    .FilterByPageVersionId(query.PageVersionId.Value)
                    .FirstOrDefaultAsync();
            }
            else
            {
                var queryResult = await _dbContext
                    .PagePublishStatusQueries
                    .AsNoTracking()
                    .Include(q => q.PageVersion)
                    .ThenInclude(v => v.Page)
                    .Include(q => q.PageVersion)
                    .ThenInclude(v => v.OpenGraphImageAsset)
                    .Include(q => q.PageVersion)
                    .ThenInclude(v => v.PageTemplate)
                    .ThenInclude(t => t.PageTemplateRegions)
                    .FilterActive()
                    .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                    .FilterByPageId(query.PageId)
                    .FirstOrDefaultAsync();

                result = queryResult?.PageVersion;
            }

            return result;
        }

        private IQueryable<PageVersionBlock> QueryPageBlocks(PageRenderDetails page)
        {
            return _dbContext
                .PageVersionBlocks
                .FilterActive()
                .AsNoTracking()
                .Where(m => m.PageVersionId == page.PageVersionId);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRenderDetailsByIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
