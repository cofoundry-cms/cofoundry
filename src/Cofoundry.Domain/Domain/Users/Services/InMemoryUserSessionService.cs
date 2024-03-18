﻿namespace Cofoundry.Domain.Internal;

/// <summary>
/// In-memory implementation of IUserSessionService for non-web
/// scenarios.
/// </summary>
/// <inheritdoc/>
public class InMemoryUserSessionService : IUserSessionService
{
    private const string AMBIENT_USER_AREA_KEY = "AMBIENT_KEY";
    private Dictionary<string, int?> _userIdCache = new Dictionary<string, int?>();
    private object _lock = new object();
    private string _ambientUserAreaCode;

    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IUserContextCache _userContextCache;

    public InMemoryUserSessionService(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IUserContextCache userContextCache
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _userContextCache = userContextCache;

        ResetAmbientUserAreaToDefault();
    }

    public int? GetCurrentUserId()
    {
        return _userIdCache.GetValueOrDefault(AMBIENT_USER_AREA_KEY);
    }

    public Task<int?> GetUserIdByUserAreaCodeAsync(string userAreaCode)
    {
        var result = GetUserIdByUserAreaCode(userAreaCode);
        return Task.FromResult(result);
    }

    public int? GetUserIdByUserAreaCode(string userAreaCode)
    {
        ArgumentNullException.ThrowIfNull(userAreaCode);

        return _userIdCache.GetValueOrDefault(userAreaCode);
    }

    public Task SignInAsync(string userAreaCode, int userId, bool rememberUser)
    {
        ArgumentNullException.ThrowIfNull(userAreaCode);
        if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId));

        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);
        EntityNotFoundException.ThrowIfNull(userArea, userAreaCode);
        var isAmbientUserArea = IsAmbientUserArea(userArea);

        lock (_lock)
        {
            _userIdCache[userArea.UserAreaCode] = userId;

            if (isAmbientUserArea)
            {
                _userIdCache[AMBIENT_USER_AREA_KEY] = userId;
            }
        }

        return Task.CompletedTask;
    }

    public Task SignOutAsync(string userAreaCode)
    {
        ArgumentNullException.ThrowIfNull(userAreaCode);

        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);
        EntityNotFoundException.ThrowIfNull(userArea, userAreaCode);
        var isAmbientUserArea = IsAmbientUserArea(userArea);

        var userId = _userIdCache.GetOrDefault(userArea.UserAreaCode);

        lock (_lock)
        {
            _userIdCache.Remove(userArea.UserAreaCode);

            if (isAmbientUserArea)
            {
                _userIdCache.Remove(AMBIENT_USER_AREA_KEY);
            }
        }

        if (userId.HasValue)
        {
            _userContextCache.Clear(userId.Value);
        }
        else
        {
            // If for whatever reason the userId wasn't set, we
            // should clear all contexts to be safe.
            _userContextCache.Clear();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Logs the user out of all user areas.
    /// </summary>
    public Task SignOutOfAllUserAreasAsync()
    {
        lock (_lock)
        {
            _userIdCache.Clear();
        }

        _userContextCache.Clear();

        return Task.CompletedTask;
    }

    public Task SetAmbientUserAreaAsync(string userAreaCode)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(userAreaCode);

        if (_ambientUserAreaCode == userAreaCode) return Task.CompletedTask;

        lock (_lock)
        {
            _ambientUserAreaCode = userAreaCode;
            var existingUserId = _userIdCache.GetValueOrDefault(userAreaCode);
            _userIdCache[AMBIENT_USER_AREA_KEY] = existingUserId;
        }

        return Task.CompletedTask;
    }

    [MemberNotNull(nameof(_ambientUserAreaCode))]
    private void ResetAmbientUserAreaToDefault()
    {
        var defaultUserArea = _userAreaDefinitionRepository.GetDefault();
        EntityNotFoundException.ThrowIfNull(defaultUserArea, "Default");

        _ambientUserAreaCode = defaultUserArea.UserAreaCode;
    }

    private bool IsAmbientUserArea(IUserAreaDefinition userArea)
    {
        return _ambientUserAreaCode == userArea.UserAreaCode;
    }

    public Task RefreshAsync(string userAreaCode, int userId)
    {
        // No-op: nothing to refresh
        return Task.CompletedTask;
    }
}
