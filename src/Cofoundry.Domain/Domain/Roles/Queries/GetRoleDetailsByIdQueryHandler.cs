namespace Cofoundry.Domain.Internal;

/// <summary>
/// <para>
/// Finds a role by it's database id, returning a <see cref="RoleDetails"/> projection 
/// if it is found, otherwise <see langword="null"/>. If no role id is specified then the 
/// anonymous role is returned.
/// </para>
/// <para>
/// Roles are cached, so repeat uses of this query is inexpensive.
/// </para>
/// </summary>
public class GetRoleDetailsByIdQueryHandler
    : IQueryHandler<GetRoleDetailsByIdQuery, RoleDetails>
    , IPermissionRestrictedQueryHandler<GetRoleDetailsByIdQuery, RoleDetails>
{
    private readonly IInternalRoleRepository _internalRoleRepository;

    public GetRoleDetailsByIdQueryHandler(
        IInternalRoleRepository internalRoleRepository
        )
    {
        _internalRoleRepository = internalRoleRepository;
    }

    public Task<RoleDetails> ExecuteAsync(GetRoleDetailsByIdQuery query, IExecutionContext executionContext)
    {
        return _internalRoleRepository.GetByIdAsync(query.RoleId);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetRoleDetailsByIdQuery command)
    {
        // Ignore permission for anonymous role.
        if (command.RoleId.HasValue)
        {
            yield return new RoleReadPermission();
        }
    }
}
