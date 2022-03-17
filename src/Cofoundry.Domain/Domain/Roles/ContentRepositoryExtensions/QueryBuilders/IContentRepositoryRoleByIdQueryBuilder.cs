namespace Cofoundry.Domain;

/// <summary>
/// Finds a role by it's database id, returning <see langword="null"/> if the role could 
/// not be found. If no role id is specified in the query then the anonymous 
/// role is returned.
/// </summary>
public interface IContentRepositoryRoleByIdQueryBuilder
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
    IDomainRepositoryQueryContext<RoleMicroSummary> AsMicroSummary();

    /// <summary>
    /// <para>
    /// <see cref="RoleDetails"/> is a fully featured role projection including all properties and 
    /// permission information.
    /// </para>
    /// <para>
    /// Roles are cached, so repeat uses of this query is inexpensive.
    /// </para>
    /// </summary>
    IDomainRepositoryQueryContext<RoleDetails> AsDetails();
}
