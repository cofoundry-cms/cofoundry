using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over role data access queries/commands to them more discoverable.
    /// </summary>
    public interface IUserRepository
    {
        #region queries

        #region get by identifier

        /// <summary>
        /// Finds a user with a specific email address in a specific user area 
        /// returning null if the user could not be found. Note that if the user
        /// area does not support email addresses then the email field will be empty.
        /// </summary>
        /// <param name="email">The email address to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        UserMicroSummary GetUserMicroSummaryByEmail(string email, string userAreaCode);

        /// <summary>
        /// Finds a user with a specific email address in a specific user area 
        /// returning null if the user could not be found. Note that if the user
        /// area does not support email addresses then the email field will be empty.
        /// </summary>
        /// <param name="email">The email address to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        Task<UserMicroSummary> GetUserMicroSummaryByEmailAsync(string email, string userAreaCode);

        /// <summary>
        /// Finds a user with a specific username address in a specific user area 
        /// returning null if the user could not be found. Note that depending on the
        /// user area, the username may be a copy of the email address.
        /// </summary>
        /// <param name="username">The username to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        UserMicroSummary GetUserMicroSummaryByUsername(string username, string userAreaCode);

        /// <summary>
        /// Finds a user with a specific username address in a specific user area 
        /// returning null if the user could not be found. Note that depending on the
        /// user area, the username may be a copy of the email address.
        /// </summary>
        /// <param name="username">The username to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        Task<UserMicroSummary> GetUserMicroSummaryByUsernameAsync(string username, string userAreaCode);

        /// <summary>
        /// Finds a user by a database id returning a UserDetails object if it 
        /// is found, otherwise null.
        /// </summary>
        /// <param name="userId">The database id of the user to find.</param>
        Task<UserDetails> GetUserDetailsByIdAsync(int userId);

        /// <summary>
        /// Finds a user by a database id returning a UserDetails object if it 
        /// is found, otherwise null.
        /// </summary>
        UserMicroSummary GetUserMicroSummaryById(int userId);

        #endregion

        #region get current user

        /// <summary>
        /// Finds a user by a database id returning a UserDetails object if it 
        /// is found, otherwise null.
        /// </summary>
        Task<UserMicroSummary> GetUserMicroSummaryByIdAsync(int userId);

        /// <summary>
        /// Gets a UserMicroSummary object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        UserMicroSummary GetCurrentUserMicroSummary();

        /// <summary>
        /// Gets a UserMicroSummary object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        Task<UserMicroSummary> GetCurrentUserMicroSummaryAsync();

        /// <summary>
        /// Gets a UserAccountDetails object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        Task<UserAccountDetails> GetCurrentUserAccountDetailsAsync();

        #endregion

        #region searches

        /// <summary>
        /// Seaches users based on simple filter criteria and returns a paged result. 
        /// </summary>
        /// <param name="query">Search query parameters.</param>
        Task<PagedQueryResult<UserSummary>> SearchUserSummariesAsync(SearchUserSummariesQuery query);

        #endregion

        #region utility

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <returns>True if the username is unique; otherwise false.</returns>
        bool IsUsernameUnique(IsUsernameUniqueQuery query);

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <returns>True if the username is unique; otherwise false.</returns>
        Task<bool> IsUsernameUniqueAsync(IsUsernameUniqueQuery query);

        #endregion

        #endregion

        #region commands

        /// <summary>
        /// A generic user creation command for use with Cofoundry users and
        /// other non-Cofoundry users. Does not send any email notifications.
        /// </summary>
        Task AddUserAsync(AddUserCommand command);

        /// <summary>
        /// Adds a user to the Cofoundry user area and sends a welcome notification
        /// containing a generated password which must be changed at first login.
        /// </summary>
        Task AddCofoundryUserAsync(AddCofoundryUserCommand command);

        /// <summary>
        /// A generic user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        Task UpdateUserAsync(UpdateUserCommand command);

        /// <summary>
        /// Marks a user as deleted in the database (soft delete).
        /// </summary>
        /// <param name="userId">Id of the role to delete.</param>
        Task DeleteUserAsync(int userId);

        /// <summary>
        /// Updates the user account of the currently logged in user.
        /// </summary>
        Task UpdateCurrentUserAccountAsync(UpdateCurrentUserAccountCommand command);

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// OldPassword field to authenticate the request.
        /// </summary>
        Task UpdateCurrentUserUserPasswordAsync(UpdateCurrentUserUserPasswordCommand command);

        #endregion
    }
}
