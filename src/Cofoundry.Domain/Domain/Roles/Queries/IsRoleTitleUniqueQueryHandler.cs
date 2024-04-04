using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Determines if a role title is unique within a specific UserArea.
/// Role titles only have to be unique per UserArea.
/// </summary>
public class IsRoleTitleUniqueQueryHandler
    : IQueryHandler<IsRoleTitleUniqueQuery, bool>
    , IPermissionRestrictedQueryHandler<IsRoleTitleUniqueQuery, bool>
{
    private readonly CofoundryDbContext _dbContext;

    public IsRoleTitleUniqueQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(IsRoleTitleUniqueQuery query, IExecutionContext executionContext)
    {
        if (string.IsNullOrWhiteSpace(query.Title))
        {
            return true;
        }

        var exists = await _dbContext
            .Roles
            .AsNoTracking()
            .Where(r => r.RoleId != query.RoleId && r.Title == query.Title && r.UserAreaCode == query.UserAreaCode)
            .AnyAsync();

        return !exists;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(IsRoleTitleUniqueQuery query)
    {
        yield return new RoleReadPermission();
    }
}
