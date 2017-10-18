using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetPageVersionSummariesByPageIdQueryHandler
        : IAsyncQueryHandler<GetPageVersionSummariesByPageIdQuery, IEnumerable<PageVersionSummary>>
        , IPermissionRestrictedQueryHandler<GetPageVersionSummariesByPageIdQuery, IEnumerable<PageVersionSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;

        public GetPageVersionSummariesByPageIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageTemplateMicroSummaryMapper pageTemplateMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageTemplateMapper = pageTemplateMapper;
        }

        #endregion

        #region execution
        
        public async Task<IEnumerable<PageVersionSummary>> ExecuteAsync(GetPageVersionSummariesByPageIdQuery query, IExecutionContext executionContext)
        {
            var dbVersions = await Query(query.PageId).ToListAsync();
            var versions = Map(dbVersions).ToList();

            return versions;
        }

        private IQueryable<PageVersion> Query(int id)
        {
            return _dbContext
                .PageVersions
                .AsNoTracking()
                .Include(v => v.Creator)
                .Include(v => v.PageTemplate)
                .Include(v => v.OpenGraphImageAsset)
                .Where(v => v.PageId == id && !v.IsDeleted)
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(v => v.CreateDate);
        }

        private IEnumerable<PageVersionSummary> Map(List<PageVersion> dbVersions)
        {
            foreach (var dbVersion in dbVersions)
            {
                var version = Mapper.Map<PageVersionSummary>(dbVersion);
                version.Template = _pageTemplateMapper.Map(dbVersion.PageTemplate);

                yield return version;
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionSummariesByPageIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
