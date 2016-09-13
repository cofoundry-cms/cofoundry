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
    public class GetPageRenderDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetPageRenderDetailsByIdQuery, PageRenderDetails>
        , IPermissionRestrictedQueryHandler<GetPageRenderDetailsByIdQuery, PageRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityVersionPageModuleMapper _entityVersionPageModuleMapper;
        private readonly PageMetaDataMapper _pageMetaDataMapper;

        public GetPageRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            EntityVersionPageModuleMapper entityVersionPageModuleMapper,
            PageMetaDataMapper pageMetaDataMapper
            )
        {
            _dbContext = dbContext;
            _entityVersionPageModuleMapper = entityVersionPageModuleMapper;
            _pageMetaDataMapper = pageMetaDataMapper;
        }

        #endregion

        #region public methods

        public async Task<PageRenderDetails> ExecuteAsync(GetPageRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var page = await GetPage(query);
            if (page == null) return null;

            _pageMetaDataMapper.MergeSitewideData(page.MetaData);

            var dbModules = await _dbContext
                .PageVersionModules
                .AsNoTracking()
                .Where(m => m.PageVersionId == page.PageVersionId)
                .ToListAsync();

            await _entityVersionPageModuleMapper.MapSections<PageVersionModuleRenderDetails>(dbModules, page.Sections, query.WorkFlowStatus, executionContext);

            return page;
        }

        #endregion

        #region private helpers

        private async Task<PageRenderDetails> GetPage(GetPageRenderDetailsByIdQuery query)
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

            var dbResult = await dbQuery.FirstOrDefaultAsync();
            var result = Mapper.Map<PageRenderDetails>(dbResult);
            return result;
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
