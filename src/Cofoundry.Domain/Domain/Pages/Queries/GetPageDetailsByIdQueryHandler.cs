using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core.DependencyInjection;
using System.Data.Entity;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetPageDetailsByIdQueryHandler 
        : IQueryHandler<GetByIdQuery<PageDetails>, PageDetails>
        , IAsyncQueryHandler<GetByIdQuery<PageDetails>, PageDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageDetails>, PageDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public GetPageDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public PageDetails Execute(GetByIdQuery<PageDetails> query, IExecutionContext executionContext)
        {
            var page = GetPageById(query.Id).FirstOrDefault();
            var sections = _queryExecutor.Execute(GetSectionQuery(page));

            return Map(query, page, sections);
        }

        public async Task<PageDetails> ExecuteAsync(GetByIdQuery<PageDetails> query, IExecutionContext executionContext)
        {
            var page = await GetPageById(query.Id).FirstOrDefaultAsync();
            var sections = await _queryExecutor.ExecuteAsync(GetSectionQuery(page));

            return Map(query, page, sections);
        }

        #endregion

        #region helpers

        private GetPageSectionDetailsByPageVersionIdQuery GetSectionQuery(PageVersion page)
        {
            var sectionQuery = new GetPageSectionDetailsByPageVersionIdQuery(page.PageVersionId);

            return sectionQuery;
        }

        private PageDetails Map(
            GetByIdQuery<PageDetails> query, 
            PageVersion dbPageVersion,
            IEnumerable<PageSectionDetails> sections
            )
        {
            if (dbPageVersion == null) return null;

            var page = Mapper.Map<PageDetails>(dbPageVersion.Page);
            Mapper.Map(dbPageVersion, page);

            // Custom Mapping
            page.PageRoute = _queryExecutor.GetById<PageRoute>(query.Id);
            EntityNotFoundException.ThrowIfNull(page, query.Id);

            page.LatestVersion.Sections = sections;

            return page;
        }

        private IOrderedQueryable<PageVersion> GetPageById(int id)
        {
            return _dbContext
                .PageVersions
                .Include(v => v.Creator)
                .Include(v => v.PageTemplate)
                .Include(v => v.Page)
                .Include(v => v.Page.Creator)
                .Include(v => v.Page.PageTags)
                .Include(v => v.Page.PageTags.Select(t => t.Tag))
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
