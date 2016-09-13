using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class SearchPageTemplateSummariesQueryHandler 
        : IAsyncQueryHandler<SearchPageTemplateSummariesQuery, PagedQueryResult<PageTemplateSummary>>
        , IPermissionRestrictedQueryHandler<SearchPageTemplateSummariesQuery, PagedQueryResult<PageTemplateSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public SearchPageTemplateSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution
        
        public async Task<PagedQueryResult<PageTemplateSummary>> ExecuteAsync(SearchPageTemplateSummariesQuery query, IExecutionContext executionContext)
        {
            var result = await CreateQuery(query).ToPagedResultAsync(query);
            
            return result;
        }

        #endregion

        #region helpers

        private IQueryable<PageTemplateSummary> CreateQuery(SearchPageTemplateSummariesQuery query)
        {
            var dbQuery = _dbContext
                .PageTemplates
                .AsNoTracking()
                .AsQueryable();
                        
            // Filter by group
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                dbQuery = dbQuery.Where(p => p.FileName.Contains(query.Name) || p.Description.Contains(query.Name));
            }

            return dbQuery
                .OrderBy(p => p.FileName)
                .ProjectTo<PageTemplateSummary>();
        }


        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchPageTemplateSummariesQuery query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
