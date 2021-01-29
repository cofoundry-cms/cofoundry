using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Search page data returning the PageSummary projection, which is primarily used
    /// to display lists of page information in the admin panel. The query isn't version 
    /// specific and should not be used to render content out to a live page because some of
    /// the pages returned may be unpublished.
    /// </summary>
    public class SearchPageSummariesQueryHandler 
        : IQueryHandler<SearchPageSummariesQuery, PagedQueryResult<PageSummary>>
        , IPermissionRestrictedQueryHandler<SearchPageSummariesQuery, PagedQueryResult<PageSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageSummaryMapper _pageSummaryMapper;

        public SearchPageSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageSummaryMapper pageSummaryMapper
            )
        {
            _dbContext = dbContext;
            _pageSummaryMapper = pageSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<PageSummary>> ExecuteAsync(SearchPageSummariesQuery query, IExecutionContext executionContext)
        {
            var dbPagedResult = await CreateQuery(query, executionContext)
                .ToPagedResultAsync(query);

            // Finish mapping children
            var mappedResults = await _pageSummaryMapper.MapAsync(dbPagedResult.Items, executionContext);

            return dbPagedResult.ChangeType(mappedResults);
        }

        private IQueryable<Page> CreateQuery(SearchPageSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = _dbContext
                .PagePublishStatusQueries
                .AsNoTracking()
                .Include(p => p.Page)
                .ThenInclude(p => p.Creator)
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

            if (!string.IsNullOrWhiteSpace(query.Text))
            {
                var sluggedQuery = SlugFormatter.ToSlug(query.Text);
                var textQuery = sluggedQuery.Replace("-", " ");

                dbQuery = dbQuery.Where(p => 
                    (p.Page.UrlPath.Contains(sluggedQuery) || (p.Page.UrlPath == string.Empty && p.Page.PageDirectory.UrlPath.Contains(sluggedQuery)))
                    || p.PageVersion.Title.Contains(textQuery));
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
                dbQuery = dbQuery.FilterByLocaleId(query.LocaleId.Value);
            }

            // Filter by directory
            if (query.PageDirectoryId > 0)
            {
                dbQuery = dbQuery.FilterByDirectoryId(query.PageDirectoryId.Value);
            }

            // Filter by group
            if (query.PageGroupId > 0)
            {
                dbQuery = dbQuery.Where(p => p.Page.PageGroupItems.Any(i => i.PageGroupId == query.PageGroupId));
            }

            return dbQuery
                .SortBy(query.SortBy, query.SortDirection)
                .Select(p => p.Page);
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
