using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetAllPageBlockTypeSummariesQueryHandler
    : IQueryHandler<GetAllPageBlockTypeSummariesQuery, IReadOnlyCollection<PageBlockTypeSummary>>
    , IPermissionRestrictedQueryHandler<GetAllPageBlockTypeSummariesQuery, IReadOnlyCollection<PageBlockTypeSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageBlockTypeCache _pageBlockTypeCache;
    private readonly IPageBlockTypeSummaryMapper _pageBlockTypeSummaryMapper;

    public GetAllPageBlockTypeSummariesQueryHandler(
        CofoundryDbContext dbContext,
        IPageBlockTypeCache pageBlockTypeCache,
        IPageBlockTypeSummaryMapper pageBlockTypeSummaryMapper
        )
    {
        _dbContext = dbContext;
        _pageBlockTypeCache = pageBlockTypeCache;
        _pageBlockTypeSummaryMapper = pageBlockTypeSummaryMapper;
    }

    public async Task<IReadOnlyCollection<PageBlockTypeSummary>> ExecuteAsync(GetAllPageBlockTypeSummariesQuery query, IExecutionContext executionContext)
    {
        return await _pageBlockTypeCache.GetOrAddAsync(async () =>
        {
            var dbResults = await Query().ToArrayAsync();
            var results = dbResults
                .Select(_pageBlockTypeSummaryMapper.Map)
                .ToArray();

            return results;
        });
    }

    private IQueryable<PageBlockType> Query()
    {
        var results = _dbContext
            .PageBlockTypes
            .AsNoTracking()
            .Include(t => t.PageBlockTemplates)
            .FilterActive()
            .OrderBy(m => m.Name);

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageBlockTypeSummariesQuery query)
    {
        yield return new PageReadPermission();
    }
}