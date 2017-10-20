using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain
{
    public class SearchPageTemplateSummariesQueryHandler 
        : IAsyncQueryHandler<SearchPageTemplateSummariesQuery, PagedQueryResult<PageTemplateSummary>>
        , IPermissionRestrictedQueryHandler<SearchPageTemplateSummariesQuery, PagedQueryResult<PageTemplateSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageTemplateSummaryMapper _pageTemplateSummaryMapper;

        public SearchPageTemplateSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageTemplateSummaryMapper pageTemplateSummaryMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageTemplateSummaryMapper = pageTemplateSummaryMapper;
        }

        #endregion

        #region execution
        
        public async Task<PagedQueryResult<PageTemplateSummary>> ExecuteAsync(SearchPageTemplateSummariesQuery query, IExecutionContext executionContext)
        {
            var dbPagedResult = await CreateQuery(query).ToPagedResultAsync(query);

            var mappedResults = dbPagedResult
                .Items
                .Select(_pageTemplateSummaryMapper.Map);

            return dbPagedResult.ChangeType(mappedResults);
        }

        #endregion

        #region helpers

        private IQueryable<PageTemplateSummaryQueryModel> CreateQuery(SearchPageTemplateSummariesQuery query)
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
                .Select(t => new PageTemplateSummaryQueryModel()
                {
                    PageTemplate = t,
                    NumPages = t.PageVersions
                        .GroupBy(p => p.PageId)
                        .Count(),
                    NumRegions = t
                        .PageTemplateRegions
                        .Count()
                });
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
