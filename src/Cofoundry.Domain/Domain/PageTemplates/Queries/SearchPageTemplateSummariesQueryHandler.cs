using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.QueryModels;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class SearchPageTemplateSummariesQueryHandler 
        : IQueryHandler<SearchPageTemplateSummariesQuery, PagedQueryResult<PageTemplateSummary>>
        , IPermissionRestrictedQueryHandler<SearchPageTemplateSummariesQuery, PagedQueryResult<PageTemplateSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageTemplateSummaryMapper _pageTemplateSummaryMapper;

        public SearchPageTemplateSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageTemplateSummaryMapper pageTemplateSummaryMapper
            )
        {
            _dbContext = dbContext;
            _pageTemplateSummaryMapper = pageTemplateSummaryMapper;
        }

        #endregion

        public async Task<PagedQueryResult<PageTemplateSummary>> ExecuteAsync(SearchPageTemplateSummariesQuery query, IExecutionContext executionContext)
        {
            var dbPagedResult = await CreateQuery(query).ToPagedResultAsync(query);
            var allPageTemplateIds = dbPagedResult
                .Items
                .Select(p => p.PageTemplate.PageTemplateId)
                .ToList();

            var templatePageCounts = await GetPageCounts(allPageTemplateIds);
            Dictionary<int, int> pageRegionCounts = await GetTemplateRegionCounts();

            foreach (var dbPagedResultItem in dbPagedResult.Items)
            {
                dbPagedResultItem.NumPages = templatePageCounts.GetOrDefault(dbPagedResultItem.PageTemplate.PageTemplateId);
                dbPagedResultItem.NumRegions = pageRegionCounts.GetOrDefault(dbPagedResultItem.PageTemplate.PageTemplateId);
            }

            var mappedResults = dbPagedResult
                .Items
                .Select(_pageTemplateSummaryMapper.Map);

            return dbPagedResult.ChangeType(mappedResults);
        }

        private async Task<Dictionary<int, int>> GetTemplateRegionCounts()
        {
            return (await _dbContext
                    .PageTemplateRegions
                    .AsNoTracking()
                    .Select(r => new { r.PageTemplateId, r.PageTemplateRegionId })
                    .ToListAsync())
                    .GroupBy(r => r.PageTemplateId)
                    .Select(g => new
                    {
                        PageTemplateId = g.Key,
                        NumRegions = g.Count()
                    })
                    .ToDictionary(r => r.PageTemplateId, r => r.NumRegions);
        }

        private async Task<Dictionary<int, int>> GetPageCounts(List<int> allPageTemplateIds)
        {
            return (await _dbContext
                    .PageVersions
                    .AsNoTracking()
                    .FilterActive()
                    .Where(v => allPageTemplateIds.Contains(v.PageTemplateId))
                    .Select(v => new { v.PageId, v.PageVersionId, v.PageTemplateId })
                    .ToListAsync())
                    .GroupBy(v => v.PageTemplateId)
                    .Select(v => new
                    {
                        PageTemplateId = v.Key,
                        NumPages = v.GroupBy(p => p.PageId).Count()
                    })
                    .ToDictionary(v => v.PageTemplateId, v => v.NumPages);
        }

        private IQueryable<PageTemplateSummaryQueryModel> CreateQuery(SearchPageTemplateSummariesQuery query)
        {
            var dbQuery = _dbContext
                .PageTemplates
                .AsNoTracking()
                .FilterActive();
                        
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
                    //NumPages = t.PageVersions
                    //    .GroupBy(p => p.PageId)
                    //    .Count(),
                    //NumRegions = t
                    //    .PageTemplateRegions
                    //    .Count()
                });
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchPageTemplateSummariesQuery query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
