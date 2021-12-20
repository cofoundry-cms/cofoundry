using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the User entity.
    /// </summary>
    public interface IAdvancedContentRepositoryUserRepository
    {
        /// <summary>
        /// Retrieve the currently logged in user. If
        /// there are multiple users then this only applies to the
        /// UserArea set as the default schema.
        /// </summary>
        IContentRepositoryCurrentUserQueryBuilder GetCurrent();

        /// <summary>
        /// Retrieve a user by a unique database id.
        /// </summary>
        /// <param name="userId">UserId of the user to get.</param>
        IContentRepositoryUserByIdQueryBuilder GetById(int userId);

        /// <summary>
        /// Search for users, returning paged lists of data.
        /// </summary>
        IContentRepositoryUserSearchQueryBuilder Search();

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per user area.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<bool> IsUsernameUnique(IsUsernameUniqueQuery query);

        /// <summary>
        /// Determines if an email address is unique within a user area. Email
        /// addresses must be unique per user area and can therefore appear in multiple
        /// user areas.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<bool> IsEmailUnique(IsEmailUniqueQuery query);

        /// <summary>
        /// A basic user creation command that adds data only and does not 
        /// send any email notifications.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created user.</returns>
        Task<int> AddAsync(AddUserCommand command);

        /// <summary>
        /// Adds a new user and sends a notification containing a generated 
        /// password which must be changed at first login.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created user.</returns>
        Task<int> AddWithTemporaryPasswordAsync(AddUserWithTemporaryPasswordCommand command);

        /// <summary>
        /// A generic user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdateUserCommand command);

        /// <summary>
        /// Marks a user as deleted in the database (soft delete).
        /// </summary>
        /// <param name="userId">UserId of the role to delete.</param>
        Task DeleteAsync(int userId);

        /// <summary>
        /// Resets a users password to a randomly generated temporary value
        /// and sends it in a mail a notification to the user. The password
        /// will need to be changed at first login (if the user area supports 
        /// it). This is designed to be used from an admin screen rather than 
        /// a self-service reset which can be done via 
        /// <see cref="InitiatePasswordResetRequest"/>.
        /// </summary>
        /// <param name="userId">Required. The database id of the user to reset the password to.</param>
        Task ResetPasswordAsync(int userId);

        /// <summary>
        /// Updates the user account of the currently logged in user.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateCurrentUserAccountAsync(UpdateCurrentUserAccountCommand command);

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// OldPassword field to authenticate the request.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateCurrentUserPasswordAsync(UpdateCurrentUserPasswordCommand command);

        /// <summary>
        /// Users can initiate self-service password reset requests that
        /// are verified by sending a message with a unique link, typically 
        /// via email.
        /// </summary>
        IAdvancedContentRepositoryUserPasswordResetRequestsRepository PasswordResetRequests();
    }
}
