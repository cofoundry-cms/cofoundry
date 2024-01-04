using Cofoundry.Core.Caching;
using System.Collections.ObjectModel;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IRoleCache"/>.
/// </summary>
public class RoleCache : IRoleCache
{
    private const string ROLE_CODE_LOOKUP_CACHEKEY = "RoleCodes";
    private const string ROLE_DETAILS_CACHEKEY = "RoleDetails";
    private const string ANON_ROLE_CACHEKEY = "AnonymousRole";
    private const string CACHEKEY = "COF_Roles";

    private readonly IObjectCache _cache;

    public RoleCache(IObjectCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Get(CACHEKEY);
    }

    /// <inheritdoc/>
    public virtual async Task<ReadOnlyDictionary<string, int>> GetOrAddRoleCodeLookupAsync(Func<Task<ReadOnlyDictionary<string, int>>> getter)
    {
        var result = await _cache.GetOrAddAsync(ROLE_CODE_LOOKUP_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAddAsync)} with key {ROLE_CODE_LOOKUP_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public virtual RoleDetails? GetOrAdd(int roleId, Func<RoleDetails?> getter)
    {
        var cacheKey = CreateRoleCacheKey(roleId);
        var result = _cache.GetOrAdd(cacheKey, getter);

        return result;
    }

    /// <inheritdoc/>
    public virtual async Task<RoleDetails?> GetOrAddAsync(int roleId, Func<Task<RoleDetails?>> getter)
    {
        var cacheKey = ROLE_DETAILS_CACHEKEY + roleId;
        var result = await _cache.GetOrAddAsync(cacheKey, getter);

        return result;
    }

    /// <inheritdoc/>
    public virtual async Task<IDictionary<int, RoleDetails>> GetOrAddRangeAsync(
        IEnumerable<int> roleIds,
        Func<IEnumerable<int>, Task<IReadOnlyCollection<RoleDetails>>> missingRolesGetter
        )
    {
        var missingIds = new HashSet<int>();
        var result = new Dictionary<int, RoleDetails>();

        foreach (var roleId in roleIds)
        {
            if (result.ContainsKey(roleId)) continue;

            var role = _cache.Get<RoleDetails>(CreateRoleCacheKey(roleId));

            if (role != null)
            {
                result.Add(roleId, role);
            }
            else if (!missingIds.Contains(roleId))
            {
                missingIds.Add(roleId);
            }
        }

        if (missingIds.Any())
        {
            var missingRoles = await missingRolesGetter.Invoke(missingIds);

            foreach (var role in missingRoles)
            {
                if (result.ContainsKey(role.RoleId)) continue;

                _cache.GetOrAdd(CreateRoleCacheKey(role.RoleId), () => role);
                result.Add(role.RoleId, role);
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public virtual RoleDetails GetOrAddAnonymousRole(Func<RoleDetails> getter)
    {
        var result = _cache.GetOrAdd(ANON_ROLE_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAdd)} with key {ANON_ROLE_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public virtual async Task<RoleDetails> GetOrAddAnonymousRoleAsync(Func<Task<RoleDetails>> getter)
    {
        var result = await _cache.GetOrAddAsync(ANON_ROLE_CACHEKEY, getter);

        if (result == null)
        {
            throw new InvalidOperationException($"Result of {nameof(_cache.GetOrAddAsync)} with key {ANON_ROLE_CACHEKEY} should never be null.");
        }

        return result;
    }

    /// <inheritdoc/>
    public virtual void Clear()
    {
        _cache.Clear();
    }

    /// <inheritdoc/>
    public virtual void Clear(int roleId)
    {
        var anonymousRole = _cache.Get<RoleDetails>(ANON_ROLE_CACHEKEY);
        if (anonymousRole?.RoleId == roleId)
        {
            _cache.Clear(ANON_ROLE_CACHEKEY);
        }
        _cache.Clear(ROLE_DETAILS_CACHEKEY + roleId);
    }

    protected virtual string CreateRoleCacheKey(int? roleId)
    {
        return ROLE_DETAILS_CACHEKEY + roleId;
    }
}
