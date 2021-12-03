namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the User entity.
    /// </summary>
    public interface IContentRepositoryUserRepository
    {
        /// <summary>
        /// Retrieve the currently logged in user. If there are multiple users then 
        /// this only applies to the user area set as the ambient auth scheme.
        /// </summary>
        IContentRepositoryCurrentUserQueryBuilder GetCurrent();

        /// <summary>
        /// Retrieve a user by a unique database id.
        /// </summary>
        /// <param name="userId">UserId of the user to get.</param>
        IContentRepositoryUserByIdQueryBuilder GetById(int userId);

        /// <summary>
        /// Query a user by their email address. Note that if the user area does not use email 
        /// addresses as the username then the email field is optional and may be empty.
        /// </summary>
        /// <param name="userAreaCode">
        /// The user area to filter on. Emails are unique per user area.
        /// </param>
        /// <param name="emailAddress">
        /// The email address to use to locate the user. The value will be normalized
        /// before making the comparison.
        /// </param>
        IContentRepositoryUserByEmailQueryBuilder GetByEmail(string userAreaCode, string emailAddress);

        /// <summary>
        /// Query a user by their unique username. A user always has a username, however it may just
        /// be a copy of the email address if the <see cref="IUserAreaDefinition.UseEmailAsUsername"/>
        /// setting is set to true.
        /// </summary>
        /// <param name="userAreaCode">
        /// The user area to filter on. Usernames are unique per user area.
        /// </param>
        /// <param name="username">
        /// The username to use to locate the user. The value will be normalized
        /// before making the comparison.
        /// </param>
        IContentRepositoryUserByUsernameQueryBuilder GetByUsername(string userAreaCode, string username);

        /// <summary>
        /// Search for users, returning paged lists of data.
        /// </summary>
        IContentRepositoryUserSearchQueryBuilder Search();
    }
}
