using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Gets a range of pages by their ids projected as PageRenderDetails models. A PageRenderDetails contains 
/// the data required to render a page, including template data for all the content-editable regions.
/// </summary>
public class GetPageRenderDetailsByIdRangeQueryHandler
    : IQueryHandler<GetPageRenderDetailsByIdRangeQuery, IReadOnlyDictionary<int, PageRenderDetails>>
    , IPermissionRestrictedQueryHandler<GetPageRenderDetailsByIdRangeQuery, IReadOnlyDictionary<int, PageRenderDetails>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageRenderDetailsMapper _pageMapper;
    private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;

    public GetPageRenderDetailsByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPageRenderDetailsMapper pageMapper,
        IEntityVersionPageBlockMapper entityVersionPageBlockMapper
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _pageMapper = pageMapper;
        _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
    }

    public async Task<IReadOnlyDictionary<int, PageRenderDetails>> ExecuteAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbPages = await GetPagesAsync(query, executionContext);
        var pageRoutes = await GetPageRoutesAsync(dbPages, executionContext);

        var pages = dbPages
            .Select(p => _pageMapper.Map(p, pageRoutes))
            .ToArray();

        var dbPageBlocks = await GetPageBlocksAsync(pages);
        var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

        await _entityVersionPageBlockMapper.MapRegionsAsync(
            dbPageBlocks,
            pages.SelectMany(p => p.Regions),
            allBlockTypes,
            query.PublishStatus,
            executionContext
            );

        return pages.ToImmutableDictionary(d => d.PageId);
    }

    private async Task<IReadOnlyDictionary<int, PageRoute>> GetPageRoutesAsync(IEnumerable<PageVersion> dbPages, IExecutionContext executionContext)
    {
        var pageIds = dbPages.Select(p => p.PageId);
        var pageRoutesQuery = new GetPageRoutesByIdRangeQuery(pageIds);
        var pageRoutes = await _queryExecutor.ExecuteAsync(pageRoutesQuery, executionContext);

        return pageRoutes;
    }

    private async Task<IEnumerable<PageVersion>> GetPagesAsync(GetPageRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
    {
        if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
        {
            throw new InvalidOperationException($"{nameof(PublishStatusQuery)}.{nameof(PublishStatusQuery.SpecificVersion)} not supported in {nameof(GetPageRenderDetailsByIdRangeQuery)}");
        }

        var dbResults = await _dbContext
            .PagePublishStatusQueries
            .AsNoTracking()
            .Include(v => v.PageVersion)
            .ThenInclude(v => v.Page)
            .Include(v => v.PageVersion)
            .ThenInclude(v => v.OpenGraphImageAsset)
            .Include(v => v.PageVersion)
            .ThenInclude(v => v.PageTemplate)
            .ThenInclude(t => t.PageTemplateRegions)
            .FilterActive()
            .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
            .Where(v => query.PageIds.Contains(v.PageId))
            .ToArrayAsync();

        return dbResults
            .Select(r => r.PageVersion);
    }

    private async Task<IReadOnlyCollection<PageVersionBlock>> GetPageBlocksAsync(IReadOnlyCollection<PageRenderDetails> pages)
    {
        var versionIds = pages.Select(p => p.PageVersionId);

        return await _dbContext
            .PageVersionBlocks
            .AsNoTracking()
            .FilterActive()
            .Where(m => versionIds.Contains(m.PageVersionId))
            .ToArrayAsync();
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRenderDetailsByIdRangeQuery query)
    {
        yield return new PageReadPermission();
    }
}
