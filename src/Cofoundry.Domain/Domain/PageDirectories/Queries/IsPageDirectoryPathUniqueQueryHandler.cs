using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Determines if a page directory UrlPath is unique
/// within its parent directory.
/// </summary>
public class IsPageDirectoryPathUniqueQueryHandler
    : IQueryHandler<IsPageDirectoryPathUniqueQuery, bool>
    , IPermissionRestrictedQueryHandler<IsPageDirectoryPathUniqueQuery, bool>
{
    private readonly CofoundryDbContext _dbContext;

    public IsPageDirectoryPathUniqueQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(IsPageDirectoryPathUniqueQuery query, IExecutionContext executionContext)
    {
        var exists = await _dbContext
            .PageDirectories
            .AsNoTracking()
            .Where(d => d.PageDirectoryId != query.PageDirectoryId
                && d.UrlPath == query.UrlPath
                && d.ParentPageDirectoryId == query.ParentPageDirectoryId
                )
            .AnyAsync();

        return !exists;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(IsPageDirectoryPathUniqueQuery command)
    {
        yield return new PageDirectoryReadPermission();
    }
}

