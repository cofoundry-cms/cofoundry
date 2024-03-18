﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Finds pages with the specified page ids and returns them as PageSummary 
/// objects. Note that this query does not account for WorkFlowStatus and so
/// pages will be returned irrecpective of whether they aree published or not.
/// </summary>
public class GetPageSummariesByIdRangeQueryHandler
    : IQueryHandler<GetPageSummariesByIdRangeQuery, IReadOnlyDictionary<int, PageSummary>>
    , IIgnorePermissionCheckHandler
{
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

    public async Task<IReadOnlyDictionary<int, PageSummary>> ExecuteAsync(GetPageSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbResult = await QueryPages(query, executionContext);

        var mappedResult = await _pageSummaryMapper.MapAsync(dbResult, executionContext);
        var dictionary = mappedResult.ToImmutableDictionary(d => d.PageId);

        return dictionary;
    }

    private async Task<IReadOnlyCollection<Page>> QueryPages(GetPageSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var results = await _dbContext
            .PagePublishStatusQueries
            .AsNoTracking()
            .Include(p => p.Page)
            .ThenInclude(p => p.Creator)
            .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
            .FilterActive()
            .Where(p => query.PageIds.Contains(p.PageId))
            .Select(p => p.Page)
            .ToArrayAsync();

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(SearchPageSummariesQuery query)
    {
        yield return new PageReadPermission();
    }
}
