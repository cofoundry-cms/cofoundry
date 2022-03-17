namespace Cofoundry.Domain;

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
public class GetRoleMicroSummaryByIdQuery : IQuery<RoleMicroSummary>
{
    public GetRoleMicroSummaryByIdQuery()
    {
    }

    /// <summary>
    /// Initializes the query with the specified role id.
    /// </summary>
    /// <param name="roleId">Database id of the role, or <see langword="null"/> to return the anonymous role.</param>
    public GetRoleMicroSummaryByIdQuery(int? roleId)
    {
        RoleId = roleId;
    }

    /// <summary>
    /// Database id of the role, or <see langword="null"/> to return the anonymous role.
    /// </summary>
    public int? RoleId { get; set; }
}
