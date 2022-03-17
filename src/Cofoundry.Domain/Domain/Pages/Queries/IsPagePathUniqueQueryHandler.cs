using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Determines if a page path already exists. Page paths are made
/// up of a locale, directory and url slug; duplicates are not permitted.
/// </summary>
public class IsPagePathUniqueQueryHandler
    : IQueryHandler<IsPagePathUniqueQuery, bool>
    , IPermissionRestrictedQueryHandler<IsPagePathUniqueQuery, bool>
{
    private readonly CofoundryDbContext _dbContext;

    public IsPagePathUniqueQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(IsPagePathUniqueQuery query, IExecutionContext executionContext)
    {
        var exists = await _dbContext
            .Pages
            .AsNoTracking()
            .Where(d => d.PageId != query.PageId
                && d.UrlPath == query.UrlPath
                && d.LocaleId == query.LocaleId
                && d.PageDirectoryId == query.PageDirectoryId
                ).AnyAsync();

        return !exists;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(IsPagePathUniqueQuery query)
    {
        yield return new PageReadPermission();
    }
}

