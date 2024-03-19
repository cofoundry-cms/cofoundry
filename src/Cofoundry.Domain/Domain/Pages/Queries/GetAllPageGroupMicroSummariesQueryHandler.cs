using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

[Obsolete("The page grouping system will be revised in an upcomming release.")]
public class GetAllPageGroupMicroSummariesQueryHandler
    : IQueryHandler<GetAllPageGroupMicroSummariesQuery, IReadOnlyCollection<PageGroupMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetAllPageGroupMicroSummariesQuery, IReadOnlyCollection<PageGroupMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;

    public GetAllPageGroupMicroSummariesQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<PageGroupMicroSummary>> ExecuteAsync(GetAllPageGroupMicroSummariesQuery query, IExecutionContext executionContext)
    {
        var results = await _dbContext
            .PageGroups
            .AsNoTracking()
            .OrderBy(m => m.GroupName)
            .Select(g => new PageGroupMicroSummary()
            {
                Name = g.GroupName,
                PageGroupId = g.PageGroupId,
                ParentGroupId = g.ParentGroupId
            })
            .ToArrayAsync();

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageGroupMicroSummariesQuery query)
    {
        yield return new PageReadPermission();
    }
}