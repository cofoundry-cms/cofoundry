using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class SearchPageSummariesQueryHandler 
        : IAsyncQueryHandler<SearchPageSummariesQuery, PagedQueryResult<PageSummary>>
        , IPermissionRestrictedQueryHandler<SearchPageSummariesQuery, PagedQueryResult<PageSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly PageSummaryMapper _pageSummaryMapper;

        public SearchPageSummariesQueryHandler(
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

        public async Task<PagedQueryResult<PageSummary>> ExecuteAsync(SearchPageSummariesQuery query, IExecutionContext executionContext)
        {
            var result = await CreateQuery(query).ToPagedResultAsync(query);

            // Finish mapping children
            await _pageSummaryMapper.MapAsync(result.Items);

            return result;
        }

        #endregion

        #region helpers

        private IQueryable<PageSummary> CreateQuery(SearchPageSummariesQuery query)
        {
            var dbQuery = _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.WebDirectory.IsActive);


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
            if (query.WebDirectoryId > 0)
            {
                dbQuery = dbQuery.Where(p => p.WebDirectoryId == query.WebDirectoryId);
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
            return dbQuery
                .OrderByDescending(p => p.CreateDate)
                .ProjectTo<PageSummary>();
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
