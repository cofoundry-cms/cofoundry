namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a role with the specified role code, returning <see langword="null"/> if the role
    /// could not be found. Roles only have a RoleCode if they have been generated 
    /// from code rather than the GUI. For GUI generated roles use a 'get by id' 
    /// query.
    /// </summary>
    public interface IContentRepositoryRoleByCodeQueryBuilder
    {
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
}
