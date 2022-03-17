namespace Cofoundry.Domain.Internal;

/// <summary>
/// Internal repository for fetching roles which bypasses CQS and permissions infrastructure
/// to avoid circular references. Not to be used outside of Cofoundry core projects
/// </summary>
/// <remarks>
/// Not actually marked internal due to internal visibility restrictions and dependency injection
/// </remarks>
public interface IInternalRoleRepository
{
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
    /// <param name="roleId">Database id of the role, or <see langword="null"/> to return the anonymous role.</param>
    RoleDetails GetById(int? roleId);

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
    /// <param name="roleId">Database id of the role, or <see langword="null"/> to return the anonymous role.</param>
    Task<RoleDetails> GetByIdAsync(int? roleId);

    /// <summary>
    /// <para>
    /// Finds a set of roles by their database ids as a lookup of <see cref="RoleDetails"/>
    /// projections.
    /// <para>
    /// Roles are cached, so repeat uses of this query is inexpensive.
    /// </para>
    /// </summary>
    /// <param name="roleIds">Database ids of the roles to get.</param>
    Task<IDictionary<int, RoleDetails>> GetByIdRangeAsync(IEnumerable<int> roleIds);

}
