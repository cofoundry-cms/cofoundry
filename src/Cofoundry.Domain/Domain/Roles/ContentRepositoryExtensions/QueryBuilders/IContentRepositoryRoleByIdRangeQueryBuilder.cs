namespace Cofoundry.Domain;

/// <summary>
/// Finds a set of roles using a collection of database ids, returning them as a 
/// <see cref="RoleMicroSummary"/> projection.
/// </summary>
public interface IContentRepositoryRoleByIdRangeQueryBuilder
{
    /// <summary>
    /// <para>
    /// <see cref="RoleMicroSummary"/> is a minimal projection of a role with only essential 
    /// identitifcation data and is typically used as part of another entity projection.
    /// </para>
    /// <para>
    /// Roles are cached, so repeat uses of this query is inexpensive.
    /// </para>
    /// </summary>
    IDomainRepositoryQueryContext<IDictionary<int, RoleMicroSummary>> AsMicroSummaries();
}
