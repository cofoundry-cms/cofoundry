namespace Cofoundry.Domain.Internal;

/// <summary>
/// <para>
/// Finds a role by it's database id, returning a <see cref="RoleMicroSummary"/> projection 
/// if it is found, otherwise <see langword="null"/>. If no role id is specified then the 
/// anonymous role is returned.
/// </para>
/// <para>
/// Roles are cached, so repeat uses of this query is inexpensive.
/// </para>
/// </summary>
public class GetRoleMicroSummaryByIdQueryHandler
    : IQueryHandler<GetRoleMicroSummaryByIdQuery, RoleMicroSummary>
    , IPermissionRestrictedQueryHandler<GetRoleMicroSummaryByIdQuery, RoleMicroSummary>
{
    private readonly IInternalRoleRepository _internalRoleRepository;
    private readonly IRoleMicroSummaryMapper _roleMicroSummaryMapper;

    public GetRoleMicroSummaryByIdQueryHandler(
        IInternalRoleRepository internalRoleRepository,
        IRoleMicroSummaryMapper roleMicroSummaryMapper
        )
    {
        _internalRoleRepository = internalRoleRepository;
        _roleMicroSummaryMapper = roleMicroSummaryMapper;
    }

    public async Task<RoleMicroSummary> ExecuteAsync(GetRoleMicroSummaryByIdQuery query, IExecutionContext executionContext)
    {
        var roleDetails = await _internalRoleRepository.GetByIdAsync(query.RoleId);
        var result = _roleMicroSummaryMapper.Map(roleDetails);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetRoleMicroSummaryByIdQuery command)
    {
        // Ignore permission for anonymous role.
        if (command.RoleId.HasValue)
        {
            yield return new RoleReadPermission();
        }
    }
}
