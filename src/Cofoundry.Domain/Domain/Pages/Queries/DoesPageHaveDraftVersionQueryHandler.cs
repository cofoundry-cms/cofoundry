using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Determines if a page has a draft version of not. A page can only have one draft
/// version at a time.
/// </summary>
public class DoesPageHaveDraftVersionQueryHandler
    : IQueryHandler<DoesPageHaveDraftVersionQuery, bool>
    , IPermissionRestrictedQueryHandler<DoesPageHaveDraftVersionQuery, bool>
{
    private readonly CofoundryDbContext _dbContext;

    public DoesPageHaveDraftVersionQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExecuteAsync(DoesPageHaveDraftVersionQuery query, IExecutionContext executionContext)
    {
        var exists = _dbContext
            .PageVersions
            .FilterActive()
            .FilterByPageId(query.PageId)
            .AnyAsync(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);

        return exists;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(DoesPageHaveDraftVersionQuery query)
    {
        yield return new PageReadPermission();
    }
}
