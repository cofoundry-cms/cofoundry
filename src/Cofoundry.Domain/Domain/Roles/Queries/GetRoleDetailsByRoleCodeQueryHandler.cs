using Cofoundry.Domain.Data;
using System.Collections.ObjectModel;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Find a role with the specified role code, returning a <see cref="RoleDetails"/> 
/// projection if one is found, otherwise <see langword="null"/>. Roles only
/// have a RoleCode if they have been generated from code rather than the GUI. 
/// For GUI generated roles use <see cref="GetRoleDetailsByIdQuery"/>.
/// </summary>
public class GetRoleDetailsByRoleCodeQueryHandler
    : IQueryHandler<GetRoleDetailsByRoleCodeQuery, RoleDetails>
    , IPermissionRestrictedQueryHandler<GetRoleDetailsByRoleCodeQuery, RoleDetails>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IInternalRoleRepository _internalRoleRepository;
    private readonly IRoleCache _roleCache;

    public GetRoleDetailsByRoleCodeQueryHandler(
        CofoundryDbContext dbContext,
        IInternalRoleRepository internalRoleRepository,
        IRoleCache roleCache
        )
    {
        _dbContext = dbContext;
        _internalRoleRepository = internalRoleRepository;
        _roleCache = roleCache;
    }

    public async Task<RoleDetails> ExecuteAsync(GetRoleDetailsByRoleCodeQuery query, IExecutionContext executionContext)
    {
        var roleCodeLookup = await _roleCache.GetOrAddRoleCodeLookupAsync(async () =>
        {
            var roleCodes = await GetRoleCodesAsync();

            return new ReadOnlyDictionary<string, int>(roleCodes);
        });

        var id = roleCodeLookup.GetOrDefault(query.RoleCode);
        if (id <= 0) return null;

        return await _internalRoleRepository.GetByIdAsync(id);
    }

    private Task<Dictionary<string, int>> GetRoleCodesAsync()
    {
        return _dbContext
            .Roles
            .AsNoTracking()
            .Where(r => r.RoleCode != null)
            .ToDictionaryAsync(r => r.RoleCode, r => r.RoleId);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetRoleDetailsByRoleCodeQuery command)
    {
        yield return new RoleReadPermission();
    }
}
