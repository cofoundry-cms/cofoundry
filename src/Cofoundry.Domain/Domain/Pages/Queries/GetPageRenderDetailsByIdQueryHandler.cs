using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a page object that contains the data required to render a page, including template 
    /// data for all the content-editable sections.
    /// </summary>
    public class GetPageRenderDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetPageRenderDetailsByIdQuery, PageRenderDetails>
        , IPermissionRestrictedQueryHandler<GetPageRenderDetailsByIdQuery, PageRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IEntityVersionPageModuleMapper _entityVersionPageModuleMapper;

        public GetPageRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IEntityVersionPageModuleMapper entityVersionPageModuleMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _entityVersionPageModuleMapper = entityVersionPageModuleMapper;
        }

        #endregion

        #region public methods

        public async Task<PageRenderDetails> ExecuteAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbPage = await QueryPage(query).FirstOrDefaultAsync();
            if (dbPage == null) return null;
            var page = Mapper.Map<PageRenderDetails>(dbPage);
            page.PageRoute = await _queryExecutor.GetByIdAsync<PageRoute>(page.PageId, executionContext);

            var dbModules = await QueryModules(page).ToListAsync();
            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);

            await _entityVersionPageModuleMapper.MapSectionsAsync(dbModules, page.Sections, allModuleTypes, query.WorkFlowStatus);

            return page;
        }

        #endregion

        #region private helpers

        private IQueryable<PageVersion> QueryPage(GetPageRenderDetailsByIdQuery query)
        {
            IQueryable<PageVersion> dbQuery = _dbContext
                .PageVersions
                .AsNoTracking()
                .Include(v => v.Page)
                .Include(v => v.PageTemplate)
                .Include(v => v.PageTemplate.PageTemplateSections)
                .Where(v => v.PageId == query.PageId && !v.IsDeleted);

            switch (query.WorkFlowStatus)
            {
                case WorkFlowStatusQuery.Draft:
                    dbQuery = dbQuery.Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);
                    break;
                case WorkFlowStatusQuery.Published:
                    dbQuery = dbQuery.Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
                    break;
                case WorkFlowStatusQuery.Latest:
                    dbQuery = dbQuery.Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
                    dbQuery = dbQuery.OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);
                    break;
                case WorkFlowStatusQuery.PreferPublished:
                    dbQuery = dbQuery.Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
                    dbQuery = dbQuery.OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
                    break;
                case WorkFlowStatusQuery.SpecificVersion:
                    dbQuery = dbQuery.Where(v => v.PageVersionId == query.PageVersionId);
                    break;
                default:
                    throw new ArgumentException("Unknown WorkFlowStatusQuery: " + query.WorkFlowStatus);
            }

            return dbQuery;
        }

        private IQueryable<PageVersionModule> QueryModules(PageRenderDetails page)
        {
            return _dbContext
                .PageVersionModules
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
