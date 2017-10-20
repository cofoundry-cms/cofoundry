using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class SearchPageSummariesQueryHandler 
        : IAsyncQueryHandler<SearchPageSummariesQuery, PagedQueryResult<PageSummary>>
        , IPermissionRestrictedQueryHandler<SearchPageSummariesQuery, PagedQueryResult<PageSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageSummaryMapper _pageSummaryMapper;

        public SearchPageSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageSummaryMapper pageSummaryMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageSummaryMapper = pageSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<PageSummary>> ExecuteAsync(SearchPageSummariesQuery query, IExecutionContext executionContext)
        {
            var dbPagedResult = await CreateQuery(query).ToPagedResultAsync(query);

            // Finish mapping children
            var mappedResults = await _pageSummaryMapper.MapAsync(dbPagedResult.Items);

            return dbPagedResult.ChangeType(mappedResults);
        }

        private IQueryable<Page> CreateQuery(SearchPageSummariesQuery query)
        {
            var dbQuery = _dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.Creator)
                .Where(p => !p.IsDeleted && p.PageDirectory.IsActive);

            // Filter by tags
            if (!string.IsNullOrEmpty(query.Tags))
            {
                var tags = TagParser.Split(query.Tags).ToList();
                foreach (string tag in tags)
                {
                    // See http://stackoverflow.com/a/7288269/486434 for why this is copied into a new variable
                    string localTag = tag;

                    dbQuery = dbQuery.Where(p => p.PageTags
                        .Select(t => t.Tag.TagText)
                        .Contains(localTag)
                        );
                }
            }

            // Filter by workflow status (only draft and published are applicable
            if (query.WorkFlowStatus == WorkFlowStatus.Draft)
            {
                dbQuery = dbQuery.Where(p => p.PageVersions
                                                .OrderByDescending(v => v.CreateDate)
                                                .Where(v => !v.IsDeleted)
                                                .Take(1)
                                                .Any(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft));
            } else if (query.WorkFlowStatus == WorkFlowStatus.Published)
            {
                // A page might be published, but also have a draft as the latest version
                dbQuery = dbQuery.Where(p => p.PageVersions
                                                .Where(v => !v.IsDeleted)
                                                .Any(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published));
            }

            // Filter by locale 
            if (query.LocaleId > 0)
            {
                dbQuery = dbQuery.Where(p => p.LocaleId == query.LocaleId);
            }

            // Filter by directory
            if (query.PageDirectoryId > 0)
            {
                dbQuery = dbQuery.Where(p => p.PageDirectoryId == query.PageDirectoryId);
            }

            // Filter by layout
            if (query.PageTemplateId > 0)
            {
                dbQuery = dbQuery.Where(p => p.PageVersions
                                                .OrderByDescending(v => v.CreateDate)
                                                .Where(v => !v.IsDeleted)
                                                .Take(1)
                                                .Any(v => v.PageTemplateId == query.PageTemplateId));
            }

            // Filter by group
            if (query.PageGroupId > 0)
            {
                dbQuery = dbQuery.Where(p => p.PageGroupItems.Any(i => i.PageGroupId == query.PageGroupId));
            }

            return dbQuery.OrderByDescending(p => p.CreateDate);
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
