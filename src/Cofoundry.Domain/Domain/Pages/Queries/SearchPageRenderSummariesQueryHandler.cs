﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Search page data returning the PageRenderSummary projection, which is
/// a lighter weight projection designed for rendering to a site when the 
/// templates, region and block data is not required. The result is 
/// version-sensitive and defaults to returning published versions only, but
/// this behavior can be controlled by the PublishStatus query property.
/// </summary>
public class SearchPageRenderSummariesQueryHandler
    : IQueryHandler<SearchPageRenderSummariesQuery, PagedQueryResult<PageRenderSummary>>
    , IPermissionRestrictedQueryHandler<SearchPageRenderSummariesQuery, PagedQueryResult<PageRenderSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageRenderSummaryMapper _pageRenderSummaryMapper;
    private readonly IQueryExecutor _queryExecutor;

    public SearchPageRenderSummariesQueryHandler(
        CofoundryDbContext dbContext,
        IPageRenderSummaryMapper pageRenderSummaryMapper,
        IQueryExecutor queryExecutor
        )
    {
        _dbContext = dbContext;
        _pageRenderSummaryMapper = pageRenderSummaryMapper;
        _queryExecutor = queryExecutor;
    }

    public async Task<PagedQueryResult<PageRenderSummary>> ExecuteAsync(SearchPageRenderSummariesQuery query, IExecutionContext executionContext)
    {
        var dbPagedResult = await QueryPagesAsync(query, executionContext);

        var allPageIds = dbPagedResult.Items.Select(p => p.PageId);
        var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetPageRoutesByIdRangeQuery(allPageIds));

        var results = new List<PageRenderSummary>(dbPagedResult.Items.Count);

        foreach (var dbResult in dbPagedResult.Items)
        {
            var mappedResult = _pageRenderSummaryMapper.Map(dbResult, allPageRoutes);
            results.Add(mappedResult);
        }

        return dbPagedResult.ChangeType(results);
    }

    private Task<PagedQueryResult<PageVersion>> QueryPagesAsync(SearchPageRenderSummariesQuery query, IExecutionContext executionContext)
    {
        var dbQuery = _dbContext
            .PagePublishStatusQueries
            .AsNoTracking()
            .Include(v => v.PageVersion)
            .ThenInclude(v => v.OpenGraphImageAsset)
            .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
            .FilterActive()
            ;

        if (query.LocaleId > 0)
        {
            dbQuery = dbQuery.FilterByLocaleId(query.LocaleId.Value);
        }

        if (query.PageDirectoryId > 0)
        {
            dbQuery = dbQuery.FilterByDirectoryId(query.PageDirectoryId.Value);
        }

        return dbQuery
            .SortBy(query.SortBy, query.SortDirection)
            .Select(p => p.PageVersion)
            .ToPagedResultAsync(query);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(SearchPageRenderSummariesQuery query)
    {
        yield return new PageReadPermission();
    }
}
