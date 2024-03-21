﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IUserContextService"/>.
/// </summary>
public class UserContextService : IUserContextService
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IUserSessionService _userSessionService;
    private readonly UserContextMapper _userContextMapper;
    private readonly IUserContextCache _userContextCache;

    public UserContextService(
        CofoundryDbContext dbContext,
        IUserSessionService userSessionService,
        UserContextMapper userContextMapper,
        IUserContextCache userContextCache
        )
    {
        _dbContext = dbContext;
        _userSessionService = userSessionService;
        _userContextMapper = userContextMapper;
        _userContextCache = userContextCache;
    }


    /// <inheritdoc/>
    public virtual async Task<IUserContext> GetCurrentContextAsync()
    {
        var userId = _userSessionService.GetCurrentUserId();
        var userContext = await GetUserContextByIdAsync(userId);

        return userContext;
    }

    /// <inheritdoc/>
    public virtual async Task<IUserContext> GetCurrentContextByUserAreaAsync(string userAreaCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userAreaCode);

        var userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(userAreaCode);
        var userContext = await GetUserContextByIdAsync(userId);

        return userContext;
    }

    /// <inheritdoc/>
    public async Task<IUserContext> GetSystemUserContextAsync()
    {
        var userContext = await _userContextCache.GetOrAddSystemContextAsync(QuerySystemUserContextAsync);

        return userContext;
    }

    /// <summary>
    /// Queries the database for the system user and returns the result as an <see cref="IUserContext"/>
    /// projection. The result of this is cached in the <see cref="GetSystemUserContextAsync"/> method.
    /// </summary>
    protected virtual async Task<IUserContext> QuerySystemUserContextAsync()
    {
        var dbUser = await _dbContext
            .Users
            .Include(u => u.Role)
            .FilterByUserArea(CofoundryAdminUserArea.Code)
            .FilterEnabled()
            .Where(u => u.IsSystemAccount)
            .FirstOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(dbUser, nameof(dbUser.IsSystemAccount));
        var impersonatedUserContext = _userContextMapper.Map(dbUser);

        return impersonatedUserContext;
    }

    private async Task<IUserContext> GetUserContextByIdAsync(int? userId)
    {
        if (!userId.HasValue)
        {
            return UserContext.Empty;
        }

        var userContext = await _userContextCache.GetOrAddAsync(userId.Value, () => QueryUserContextByIdAsync(userId.Value));

        return userContext;
    }

    /// <summary>
    /// Queries the database for the specified user and returns the result as an <see cref="IUserContext"/>
    /// projection. The result of this is cached in the <see cref="GetUserContextByIdAsync"/> method.
    /// </summary>
    protected virtual async Task<IUserContext> QueryUserContextByIdAsync(int userId)
    {
        IUserContext? cx = null;

        // Raw query required here because using IQueryExecutor will cause a stack overflow
        var dbResult = await _dbContext
            .Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FilterById(userId)
            .FilterCanSignIn()
            .SingleOrDefaultAsync();

        if (dbResult != null)
        {
            cx = _userContextMapper.Map(dbResult);
        }
        else
        {
            cx = UserContext.Empty;

            // User no longer valid, clear out all sessions to be safe
            await _userSessionService.SignOutOfAllUserAreasAsync();
        }

        return cx;
    }
}
