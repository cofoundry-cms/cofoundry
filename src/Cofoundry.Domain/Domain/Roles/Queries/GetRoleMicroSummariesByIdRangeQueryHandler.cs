namespace Cofoundry.Domain.Internal;

/// <summary>
/// <para>
/// Finds a set of roles using a collection of database ids, returning them as a 
/// <see cref="RoleMicroSummary"/> projection.
/// </para>
/// <para>
/// Roles are cached, so repeat uses of this query is inexpensive.
/// </para>
/// </summary>
public class GetRoleMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetRoleMicroSummariesByIdRangeQuery, IDictionary<int, RoleMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetRoleMicroSummariesByIdRangeQuery, IDictionary<int, RoleMicroSummary>>
{
    private readonly IInternalRoleRepository _internalRoleRepository;
    private readonly IRoleMicroSummaryMapper _roleMicroSummaryMapper;

    public GetRoleMicroSummariesByIdRangeQueryHandler(
        IInternalRoleRepository internalRoleRepository,
        IRoleMicroSummaryMapper roleMicroSummaryMapper
        )
    {
        _internalRoleRepository = internalRoleRepository;
        _roleMicroSummaryMapper = roleMicroSummaryMapper;
    }

    public async Task<IDictionary<int, RoleMicroSummary>> ExecuteAsync(GetRoleMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        if (EnumerableHelper.IsNullOrEmpty(query.RoleIds))
        {
            return new Dictionary<int, RoleMicroSummary>();
        }

        var roleDetails = await _internalRoleRepository.GetByIdRangeAsync(query.RoleIds);

        var result = roleDetails
            .Select(d => _roleMicroSummaryMapper.Map(d.Value))
            .ToDictionary(k => k.RoleId);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetRoleMicroSummariesByIdRangeQuery command)
    {
        yield return new RoleReadPermission();
    }
}
