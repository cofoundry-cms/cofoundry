namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// Finds a set of roles using a collection of database ids, returning them as a 
/// <see cref="RoleMicroSummary"/> projection.
/// </para>
/// <para>
/// Roles are cached, so repeat uses of this query is inexpensive.
/// </para>
/// </summary>
public class GetRoleMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RoleMicroSummary>>
{
    public GetRoleMicroSummariesByIdRangeQuery()
    {
        RoleIds = new List<int>();
    }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="roleIds">Collection of database ids of the roles to get.</param>
    public GetRoleMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> roleIds
        )
    {
        ArgumentNullException.ThrowIfNull(roleIds);

        RoleIds = roleIds;
    }

    /// <summary>
    /// Collection of database ids of the roles to get.
    /// </summary>
    [Required]
    public IReadOnlyCollection<int> RoleIds { get; set; }
}
