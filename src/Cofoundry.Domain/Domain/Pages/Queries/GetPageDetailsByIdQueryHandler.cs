using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetPageDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageDetails>, PageDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageDetails>, PageDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageTemplateMapper _pageTemplateMapper;

        public GetPageDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageTemplateMapper pageTemplateMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageTemplateMapper = pageTemplateMapper;
        }

        #endregion

        #region execution
        
        public async Task<PageDetails> ExecuteAsync(GetByIdQuery<PageDetails> query, IExecutionContext executionContext)
        {
            var dbPageVersion = await GetPageById(query.Id).FirstOrDefaultAsync();
            if (dbPageVersion == null) return null;

            var pageRoute = await _queryExecutor.GetByIdAsync<PageRoute>(query.Id);
            EntityNotFoundException.ThrowIfNull(pageRoute, query.Id);

            var regions = await _queryExecutor.ExecuteAsync(GetRegionsQuery(dbPageVersion));

            return Map(dbPageVersion, regions, pageRoute);
        }

        private GetPageRegionDetailsByPageVersionIdQuery GetRegionsQuery(PageVersion page)
        {
            var regionsQuery = new GetPageRegionDetailsByPageVersionIdQuery(page.PageVersionId);

            return regionsQuery;
        }

        private PageDetails Map(
            PageVersion dbPageVersion,
            IEnumerable<PageRegionDetails> regions,
            PageRoute pageRoute
            )
        {
            var page = Mapper.Map<PageDetails>(dbPageVersion.Page);
            Mapper.Map(dbPageVersion, page);

            // Custom Mapping
            page.PageRoute = pageRoute;
            page.LatestVersion.Template = _pageTemplateMapper.MapMicroSummary(dbPageVersion.PageTemplate);
            page.LatestVersion.Regions = regions;

            return page;
        }

        private IOrderedQueryable<PageVersion> GetPageById(int id)
        {
            return _dbContext
                .PageVersions
                .Include(v => v.Creator)
                .Include(v => v.PageTemplate)
                .Include(v => v.Page)
                .ThenInclude(p => p.Creator)
                .Include(v => v.Page)
                .ThenInclude(p => p.PageTags)
                .ThenInclude(t => t.Tag)
                .Include(v => v.OpenGraphImageAsset)
                .AsNoTracking()
                .Where(v => v.PageId == id && !v.IsDeleted && !v.Page.IsDeleted)
                .OrderByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(g => g.CreateDate);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageDetails> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
