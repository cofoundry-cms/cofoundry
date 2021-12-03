namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user with a specific email address returning <see langword="null"/> 
    /// if the user could not be found. Note that if the user area does not use email 
    /// addresses as the username then the email field is optional and may be empty.
    /// </summary>
    public interface IContentRepositoryUserByEmailQueryBuilder
    {
        /// <summary>
        /// The UserMicroSummary is a minimal projection of user data that is quick
        /// to load. 
        /// </summary>
        IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary();
    }
}
