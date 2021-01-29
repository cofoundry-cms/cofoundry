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
    /// Returns a paged collection of versions of a specific page, ordered 
    /// historically with the latest/draft version first.
    /// </summary>
    public class GetPageVersionSummariesByPageIdQueryHandler
        : IQueryHandler<GetPageVersionSummariesByPageIdQuery, PagedQueryResult<PageVersionSummary>>
        , IPermissionRestrictedQueryHandler<GetPageVersionSummariesByPageIdQuery, PagedQueryResult<PageVersionSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageVersionSummaryMapper _pageVersionSummaryMapper;

        public GetPageVersionSummariesByPageIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageVersionSummaryMapper pageVersionSummaryMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageVersionSummaryMapper = pageVersionSummaryMapper;
        }

        #endregion

        #region execution
        
        public async Task<PagedQueryResult<PageVersionSummary>> ExecuteAsync(GetPageVersionSummariesByPageIdQuery query, IExecutionContext executionContext)
        {
            var dbVersions = await Query(query.PageId).ToPagedResultAsync(query);
            var versions = _pageVersionSummaryMapper.MapVersions(query.PageId, dbVersions);

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
                .FilterActive()
                .FilterByPageId(id)
                .OrderByLatest();
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
