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
    public class GetPageVersionDetailsByPageIdQueryHandler 
        : IQueryHandler<GetPageVersionDetailsByPageIdQuery, IEnumerable<PageVersionDetails>>
        , IAsyncQueryHandler<GetPageVersionDetailsByPageIdQuery, IEnumerable<PageVersionDetails>>
        , IPermissionRestrictedQueryHandler<GetPageVersionDetailsByPageIdQuery, IEnumerable<PageVersionDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public GetPageVersionDetailsByPageIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public IEnumerable<PageVersionDetails> Execute(GetPageVersionDetailsByPageIdQuery query, IExecutionContext executionContext)
        {
            var dbVersions = Query(query.PageId).ToList();
            var versions = Map(dbVersions);

            return versions;
        }

        public async Task<IEnumerable<PageVersionDetails>> ExecuteAsync(GetPageVersionDetailsByPageIdQuery query, IExecutionContext executionContext)
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

        private IEnumerable<PageVersionDetails> Map(List<PageVersion> dbVersions)
        {
            var versions = Mapper.Map<IEnumerable<PageVersionDetails>>(dbVersions);
            return versions;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionDetailsByPageIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
