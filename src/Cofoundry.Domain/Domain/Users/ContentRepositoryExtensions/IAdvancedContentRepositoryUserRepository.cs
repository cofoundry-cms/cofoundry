using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the User entity.
    /// </summary>
    public interface IAdvancedContentRepositoryUserRepository
    {
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
        /// Validates a username, returning any errors found. By default the validator checks that 
        /// the format contains only the characters permitted by the <see cref="UsernameOptions"/> 
        /// configuration settings, as well as checking for uniquness.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<ValidationQueryResult> ValidateUsername(ValidateUsernameQuery query);

        /// <summary>
        /// Determines if an email address is unique within a user area. Email
        /// addresses must be unique per user area and can therefore appear in multiple
        /// user areas.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<bool> IsEmailAddressUnique(IsUserEmailAddressUniqueQuery query);

        /// <summary>
        /// Validates a user email address, returning any errors found. By default the validator
        /// checks that the format contains only the characters permitted by the 
        /// <see cref="EmailAddressOptions"/> configuration settings, as well as checking
        /// for uniqueness if necessary.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<ValidationQueryResult> ValidateEmailAddress(ValidateUserEmailAddressQuery query);

        /// <summary>
        /// A basic user creation command that adds data only and does not 
        /// send any email notifications.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created user.</returns>
        Task<int> AddAsync(AddUserCommand command);

        /// <summary>
        /// Adds a new user and sends a notification containing a generated 
        /// password which must be changed at first sign in.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created user.</returns>
        Task<int> AddWithTemporaryPasswordAsync(AddUserWithTemporaryPasswordCommand command);

        /// <summary>
        /// A general-purpose user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdateUserCommand command);

        /// <summary>
        /// A general-purpose user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        /// <param name="userId">Database id of the user to update.</param>
        /// <param name="commandPatcher">
        /// An action to configure or "patch" a command that's been initialized
        /// with the existing user data.
        /// </param>
        Task UpdateAsync(int userId, Action<UpdateUserCommand> commandPatcher);

        /// <summary>
        /// Updates the password of an unathenticated user, using the
        /// credentials in the command to authenticate the request.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdatePasswordByCredentialsAsync(UpdateUserPasswordByCredentialsCommand command);

        /// <summary>
        /// Marks a user as deleted in the database (soft delete), removing personal 
        /// data fields and any optional relations from the UnstructuredDataDependency
        /// table. The remaining user record and relations are left in place for auditing.
        /// Log tables that contain IP references are not deleted, but should be
        /// cleared out periodically by the <see cref="BackgroundTasks.UserCleanupBackgroundTask"/>.
        /// </summary>
        /// <param name="userId">UserId of the role to delete.</param>
        Task DeleteAsync(int userId);

        /// <summary>
        /// Resets a users password to a randomly generated temporary value
        /// and sends it in a mail a notification to the user. The password
        /// will need to be changed at first sign in (if the user area supports 
        /// it). This is designed to be used from an admin screen rather than 
        /// a self-service reset which can be done via 
        /// <see cref="InitiateUserAccountRecoveryViaEmailCommand"/>.
        /// </summary>
        /// <param name="userId">Required. The database id of the user to reset the password to.</param>
        Task ResetPasswordAsync(int userId);

        /// <summary>
        /// Queries and commands relating to the currently logged in user.
        /// </summary>
        IAdvancedContentRepositoryCurrentUserRepository Current();

        /// <summary>
        /// Users can initiate self-service account recovery (AKA "forgot password") 
        /// requests that are verified by sending a message with a unique link, typically 
        /// via email.
        /// </summary>
        IAdvancedContentRepositoryUserAccountRecoveryRepository AccountRecovery();

        /// <summary>
        /// Users can verify their account via an out-of-band notification such as
        /// an email containing a unique verification link.
        /// </summary>
        IAdvancedContentRepositoryUserAccountVerificationRepository AccountVerification();

        /// <summary>
        /// Authenticate a user and log in or out.
        /// </summary>
        IAdvancedContentRepositoryUserAuthenticationRepository Authentication();
    }
}