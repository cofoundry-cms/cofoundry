using Cofoundry.Domain.Data;
using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

public class GetAllPageGroupSummariesQueryHandler
    : IQueryHandler<GetAllPageGroupSummariesQuery, ICollection<PageGroupSummary>>
    , IPermissionRestrictedQueryHandler<GetAllPageGroupSummariesQuery, ICollection<PageGroupSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageGroupSummaryMapper _pageGroupSummaryMapper;

    public GetAllPageGroupSummariesQueryHandler(
        CofoundryDbContext dbContext,
        IPageGroupSummaryMapper pageGroupSummaryMapper
        )
    {
        _dbContext = dbContext;
        _pageGroupSummaryMapper = pageGroupSummaryMapper;
    }

    public async Task<ICollection<PageGroupSummary>> ExecuteAsync(GetAllPageGroupSummariesQuery query, IExecutionContext executionContext)
    {
        var dbResults = await _dbContext
            .PageGroups
            .AsNoTracking()
            .OrderBy(m => m.GroupName)
            .Select(g => new PageGroupSummaryQueryModel()
            {
                PageGroup = g,
                Creator = g.Creator,
                NumPages = g
                    .PageGroupItems
                    .Count()
            })
            .ToListAsync();

        var results = dbResults
            .Select(_pageGroupSummaryMapper.Map)
            .ToList();

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageGroupSummariesQuery query)
    {
        yield return new PageReadPermission();
    }
}