namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user with a specific username, returning <see langword="null"/> if the 
    /// user could not be found. A user always has a username, however it may just
    /// be a copy of the email address if the <see cref="IUserAreaDefinition.UseEmailAsUsername"/>
    /// setting is set to true.
    /// </summary>
    public interface IContentRepositoryUserByUsernameQueryBuilder
    {
        /// <summary>
        /// The <see cref="UserMicroSummary"/> is a minimal projection of user data that 
        /// is quick to load. 
        /// </summary>
        IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary();
    }
}
