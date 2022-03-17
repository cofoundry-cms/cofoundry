namespace Cofoundry.Domain;

/// <summary>
/// IContentRespository extension root for the Role entity.
/// </summary>
public interface IContentRepositoryRoleRepository
{
    /// <summary>
    /// Finds a role by it's database id, returning <see langword="null"/> if the role could 
    /// not be found. If no role id is specified in the query then the anonymous 
    /// role is returned.
    /// </summary>
    /// <param name="roleId">Database id of the role, or null to return the anonymous role.</param>
    IContentRepositoryRoleByIdQueryBuilder GetById(int? roleId);

    /// <summary>
    /// Finds a role with the specified role code, returning <see langword="null"/> if the role
    /// could not be found. Roles only have a RoleCode if they have been generated 
    /// from code rather than the GUI. For GUI generated roles use a 'get by id' 
    /// query.
    /// </summary>
    /// <param name="roleCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
    IContentRepositoryRoleByCodeQueryBuilder GetByCode(string roleCode);

    /// <summary>
    /// Finds a set of roles using a collection of database ids, returning them as a 
    /// <see cref="RoleMicroSummary"/> projection.
    /// </summary>
    /// <param name="roleIds">Range of role ids of the pages to get.</param>
    IContentRepositoryRoleByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> roleIds);
}
