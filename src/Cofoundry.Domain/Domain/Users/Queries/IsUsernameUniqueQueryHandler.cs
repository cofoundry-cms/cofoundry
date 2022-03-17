using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Determines if a username is unique within a specific UserArea.
/// Usernames only have to be unique per user area.
/// </summary>
public class IsUsernameUniqueQueryHandler
    : IQueryHandler<IsUsernameUniqueQuery, bool>
    , IPermissionRestrictedQueryHandler<IsUsernameUniqueQuery, bool>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IUserDataFormatter _userDataFormatter;

    public IsUsernameUniqueQueryHandler(
        CofoundryDbContext dbContext,
        IUserDataFormatter userDataFormatter
        )
    {
        _dbContext = dbContext;
        _userDataFormatter = userDataFormatter;
    }

    public async Task<bool> ExecuteAsync(IsUsernameUniqueQuery query, IExecutionContext executionContext)
    {
        var uniqueUsername = _userDataFormatter.UniquifyUsername(query.UserAreaCode, query.Username);
        if (string.IsNullOrWhiteSpace(uniqueUsername)) return true;

        var exists = await _dbContext
            .Users
            .AsNoTracking()
            .FilterNotDeleted()
            .FilterByUserArea(query.UserAreaCode)
            .Where(u => u.UserId != query.UserId && u.UniqueUsername == uniqueUsername)
            .AnyAsync();

        return !exists;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(IsUsernameUniqueQuery query)
    {
        if (query.UserAreaCode == CofoundryAdminUserArea.Code)
        {
            yield return new CofoundryUserReadPermission();
        }
        else
        {
            yield return new NonCofoundryUserReadPermission();
        }
    }
}
