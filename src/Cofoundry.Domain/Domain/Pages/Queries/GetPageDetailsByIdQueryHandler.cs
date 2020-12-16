using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns detailed information on a page and it's latest version. This 
    /// query is primarily used in the admin area because it is not version-specific
    /// and the PageDetails projection includes audit data and other additional 
    /// information that should normally be hidden from a customer facing app.
    /// </summary>
    public class GetPageDetailsByIdQueryHandler 
        : IQueryHandler<GetPageDetailsByIdQuery, PageDetails>
        , IPermissionRestrictedQueryHandler<GetPageDetailsByIdQuery, PageDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
        private readonly IAuditDataMapper _auditDataMapper;
        private readonly IOpenGraphDataMapper _openGraphDataMapper;

        public GetPageDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageTemplateMicroSummaryMapper pageTemplateMapper,
            IAuditDataMapper auditDataMapper,
            IOpenGraphDataMapper openGraphDataMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageTemplateMapper = pageTemplateMapper;
            _auditDataMapper = auditDataMapper;
            _openGraphDataMapper = openGraphDataMapper;
        }

        #endregion

        #region execution
        
        public async Task<PageDetails> ExecuteAsync(GetPageDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbPageVersion = await GetPageById(query.PageId).FirstOrDefaultAsync();
            if (dbPageVersion == null) return null;

            var pageRouteQuery = new GetPageRouteByIdQuery(query.PageId);
            var pageRoute = await _queryExecutor.ExecuteAsync(pageRouteQuery, executionContext);
            EntityNotFoundException.ThrowIfNull(pageRoute, query.PageId);

            var regions = await _queryExecutor.ExecuteAsync(GetRegionsQuery(dbPageVersion), executionContext);

            return Map(dbPageVersion, regions, pageRoute);
        }

        private GetPageRegionDetailsByPageVersionIdQuery GetRegionsQuery(PageVersion page)
        {
            var regionsQuery = new GetPageRegionDetailsByPageVersionIdQuery(page.PageVersionId);

            return regionsQuery;
        }

        private PageDetails Map(
            PageVersion dbPageVersion,
            ICollection<PageRegionDetails> regions,
            PageRoute pageRoute
            )
        {
            var page = new PageDetails();
            page.PageId = dbPageVersion.PageId;
            page.PageRoute = pageRoute;
            page.AuditData = _auditDataMapper.MapCreateAuditData(dbPageVersion.Page);
            page.Tags = dbPageVersion
                .Page
                .PageTags
                .Select(t => t.Tag.TagText)
                .OrderBy(t => t)
                .ToList();

            page.LatestVersion = new PageVersionDetails()
            {
                MetaDescription = dbPageVersion.MetaDescription,
                PageVersionId = dbPageVersion.PageVersionId,
                ShowInSiteMap = !dbPageVersion.ExcludeFromSitemap,
                DisplayVersion = dbPageVersion.DisplayVersion,
                Title = dbPageVersion.Title,
                WorkFlowStatus = (WorkFlowStatus)dbPageVersion.WorkFlowStatusId
            };

            page.LatestVersion.OpenGraph = _openGraphDataMapper.Map(dbPageVersion);
            page.LatestVersion.Template = _pageTemplateMapper.Map(dbPageVersion.PageTemplate);
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
                .ThenInclude(v => v.Creator)
                .AsNoTracking()
                .FilterActive()
                .FilterByPageId(id)
                .OrderByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(g => g.CreateDate);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageDetailsByIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
