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
            var dbPagedResult = await CreateQuery(query, executionContext)
                .ToPagedResultAsync(query);

            // Have to refilter here because EF won't let us include related entieis after a .Select statement yet
            var items = dbPagedResult
                .Items
                .Select(p => p.Page)
                .ToList();

            // Finish mapping children
            var mappedResults = await _pageSummaryMapper.MapAsync(items, executionContext);

            return dbPagedResult.ChangeType(mappedResults);
        }

        private IQueryable<PagePublishStatusQuery> CreateQuery(SearchPageSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = _dbContext
                .PagePublishStatusQueries
                .AsNoTracking()
                .Include(p => p.Page)
                .Include(p => p.Page.Creator)
                .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
                .FilterActive()
                ;

            // Filter by layout
            if (query.PageTemplateId > 0)
            {
                dbQuery = dbQuery.Where(v => v.PageVersion.PageTemplateId == query.PageTemplateId);
            }
            
            // Filter by tags
            if (!string.IsNullOrEmpty(query.Tags))
            {
                var tags = TagParser.Split(query.Tags).ToList();
                foreach (string tag in tags)
                {
                    // See http://stackoverflow.com/a/7288269/486434 for why this is copied into a new variable
                    string localTag = tag;

                    dbQuery = dbQuery.Where(p => p.Page.PageTags
                        .Select(t => t.Tag.TagText)
                        .Contains(localTag)
                        );
                }
            }

            // Filter by workflow status (only draft and published are applicable
            if (query.PublishStatus == PublishStatus.Published)
            {
                dbQuery = dbQuery.Where(p => p.Page.PublishStatusCode == PublishStatusCode.Published);
            } else if (query.PublishStatus == PublishStatus.Unpublished)
            {
                // A page might be published, but also have a draft as the latest version
                dbQuery = dbQuery.Where(p => p.Page.PublishStatusCode == PublishStatusCode.Unpublished);
            }

            // Filter by locale 
            if (query.LocaleId > 0)
            {
                dbQuery = dbQuery.Where(p => p.Page.LocaleId == query.LocaleId);
            }

            // Filter by directory
            if (query.PageDirectoryId > 0)
            {
                dbQuery = dbQuery.Where(p => p.Page.PageDirectoryId == query.PageDirectoryId);
            }

            // Filter by group
            if (query.PageGroupId > 0)
            {
                dbQuery = dbQuery.Where(p => p.Page.PageGroupItems.Any(i => i.PageGroupId == query.PageGroupId));
            }

            return dbQuery.OrderByDescending(p => p.Page.CreateDate);
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
