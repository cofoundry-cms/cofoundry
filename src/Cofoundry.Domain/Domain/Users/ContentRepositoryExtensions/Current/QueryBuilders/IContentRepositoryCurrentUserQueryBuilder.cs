namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for accessing the currently logged in user. If
    /// there are multiple users then this only applies to the
    /// UserArea set as the default schema.
    /// </summary>
    public interface IContentRepositoryCurrentUserQueryBuilder
    {
        /// <summary>
        /// An <see cref="IUserContext"/> contains only key data about the current
        /// authenticated user such as their <see cref="IUserContext.UserId"/>, 
        /// <see cref="IUserContext.RoleId"/> and which <see cref="IUserContext.UserArea"/> 
        /// they belong to. An <see cref="IUserContext"/> is cached for the duration of the request 
        /// and is the most efficient way to quickly reference the current user. If the user 
        /// is not logged in then <see cref="UserContext.Empty"/> is returned, which represents an 
        /// anonymous user. 
        /// </summary>
        IDomainRepositoryQueryContext<IUserContext> AsUserContext();


        /// <summary>
        /// <see cref="UserMicroSummary"/> is a minimal projection of user data 
        /// that is quick to load. If the user is not logged in then <see langword="null"/>
        /// is returned.
        /// </summary>
        IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary();

        /// <summary>
        /// The <see cref="UserSummary"/> is a reduced representation of a user. Building 
        /// on the <see cref="UserMicroSummary"/>, this projection contains additional audit 
        /// and basic role data. If the user is not logged in then <see langword="null"/> 
        /// is returned.
        /// </summary>
        IDomainRepositoryQueryContext<UserSummary> AsSummary();

        /// <summary>
        /// The <see cref="UserDetails"/> projection is a full representation of a user, containing 
        /// all properties including role and permission data. If the user is not logged 
        /// in then <see langword="null"/> is returned.
        /// </summary>
        IDomainRepositoryQueryContext<UserDetails> AsDetails();
    }
}