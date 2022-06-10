namespace Cofoundry.Domain.Internal;

public class UserContextCache : IUserContextCache
{
    /// <summary>
    /// In-memory cache scoped to the lifetime of the instance, which
    /// should be per-scope for web requests.
    /// </summary>
    private Dictionary<int, IUserContext> _userContextCache = new Dictionary<int, IUserContext>();
    private const int SYSTEM_CACHE_KEY = Int32.MinValue;

    public async Task<IUserContext> GetOrAddAsync(int userId, Func<Task<IUserContext>> getter)
    {
        ArgumentNullException.ThrowIfNull(getter);
        if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId), nameof(userId) + " must be positive.");

        var userContext = _userContextCache.GetValueOrDefault(userId);

        if (userContext == null)
        {
            userContext = await getter();
            _userContextCache.TryAdd(userId, userContext);
        }

        return userContext;
    }

    public async Task<IUserContext> GetOrAddSystemContextAsync(Func<Task<IUserContext>> getter)
    {
        ArgumentNullException.ThrowIfNull(getter);

        var userContext = _userContextCache.GetValueOrDefault(SYSTEM_CACHE_KEY);

        if (userContext == null)
        {
            userContext = await getter();
            _userContextCache.TryAdd(SYSTEM_CACHE_KEY, userContext);
        }

        return userContext;
    }

    public void Clear(int userId)
    {
        if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId), nameof(userId) + " must be positive.");

        _userContextCache.Remove(userId);
    }

    public void Clear()
    {
        _userContextCache.Clear();
    }
}
