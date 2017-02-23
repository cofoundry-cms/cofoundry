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
    public class GetPageVersionSummariesByPageIdQueryHandler
        : IQueryHandler<GetPageVersionSummariesByPageIdQuery, IEnumerable<PageVersionSummary>>
        , IAsyncQueryHandler<GetPageVersionSummariesByPageIdQuery, IEnumerable<PageVersionSummary>>
        , IPermissionRestrictedQueryHandler<GetPageVersionSummariesByPageIdQuery, IEnumerable<PageVersionSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public GetPageVersionSummariesByPageIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public IEnumerable<PageVersionSummary> Execute(GetPageVersionSummariesByPageIdQuery query, IExecutionContext executionContext)
        {
            var dbVersions = Query(query.PageId).ToList();
            var versions = Map(dbVersions);

            return versions;
        }

        public async Task<IEnumerable<PageVersionSummary>> ExecuteAsync(GetPageVersionSummariesByPageIdQuery query, IExecutionContext executionContext)
        {
            var dbVersions = await Query(query.PageId).ToListAsync();
            var versions = Map(dbVersions);

            return versions;
        }

        #endregion

        #region helpers

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
            var versions = Mapper.Map<IEnumerable<PageVersionSummary>>(dbVersions);
            return versions;
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
