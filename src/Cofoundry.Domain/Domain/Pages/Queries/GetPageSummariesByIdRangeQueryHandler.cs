using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Finds pages with the specified page ids and returns them as PageSummary 
    /// objects. Note that this query does not account for WorkFlowStatus and so
    /// pages will be returned irrecpective of whether they aree published or not.
    /// </summary>
    public class GetPageSummariesByIdRangeQueryHandler
        : IQueryHandler<GetPageSummariesByIdRangeQuery, IDictionary<int, PageSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageSummaryMapper _pageSummaryMapper;

        public GetPageSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IPageSummaryMapper pageSummaryMapper
            )
        {
            _dbContext = dbContext;
            _pageSummaryMapper = pageSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, PageSummary>> ExecuteAsync(GetPageSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResult = await CreateQuery(query).ToListAsync();

            // Finish mapping children
            var mappedResult = await _pageSummaryMapper.MapAsync(dbResult, executionContext);
            var dictionary = mappedResult.ToDictionary(d => d.PageId);

            return dictionary;
        }

        private IQueryable<Page> CreateQuery(GetPageSummariesByIdRangeQuery query)
        {
            var dbQuery = _dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.Creator)
                .FilterActive()
                .Where(p => query.PageIds.Contains(p.PageId));

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchPageSummariesQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
