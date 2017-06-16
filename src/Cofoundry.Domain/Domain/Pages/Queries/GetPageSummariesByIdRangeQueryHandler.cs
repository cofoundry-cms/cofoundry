using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds pages with the specified page ids and returns them as PageSummary 
    /// objects. Note that this query does not account for WorkFlowStatus and so
    /// pages will be returned irrecpective of whether they aree published or not.
    /// </summary>
    public class GetPageSummariesByIdRangeQueryHandler
        : IAsyncQueryHandler<GetPageSummariesByIdRangeQuery, IDictionary<int, PageSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly PageSummaryMapper _pageSummaryMapper;

        public GetPageSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageSummaryMapper = new PageSummaryMapper(_dbContext, _queryExecutor);
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, PageSummary>> ExecuteAsync(GetPageSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var result = await CreateQuery(query).ToListAsync();

            // Finish mapping children
            await _pageSummaryMapper.MapAsync(result);
            var dictionary = result.ToDictionary(d => d.PageId);

            return dictionary;
        }

        #endregion

        #region helpers

        private IQueryable<PageSummary> CreateQuery(GetPageSummariesByIdRangeQuery query)
        {
            var dbQuery = _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.WebDirectory.IsActive)
                .Where(p => query.PageIds.Contains(p.PageId))
                .ProjectTo<PageSummary>();

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
