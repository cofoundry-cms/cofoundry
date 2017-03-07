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
    public class UserRepository : IUserRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public UserRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        #region get by identifier

        /// <summary>
        /// Finds a user with a specific email address in a specific user area 
        /// returning null if the user could not be found. Note that if the user
        /// area does not support email addresses then the email field will be empty.
        /// </summary>
        /// <param name="email">The email address to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        public UserMicroSummary GetUserMicroSummaryByEmail(string email, string userAreaCode)
        {
            return _queryExecutor.Execute(new GetUserMicroSummaryByEmailQuery(email, userAreaCode));
        }

        /// <summary>
        /// Finds a user with a specific email address in a specific user area 
        /// returning null if the user could not be found. Note that if the user
        /// area does not support email addresses then the email field will be empty.
        /// </summary>
        /// <param name="email">The email address to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        public Task<UserMicroSummary> GetUserMicroSummaryByEmailAsync(string email, string userAreaCode)
        {
            return _queryExecutor.ExecuteAsync(new GetUserMicroSummaryByEmailQuery(email, userAreaCode));
        }

        /// <summary>
        /// Finds a user with a specific username address in a specific user area 
        /// returning null if the user could not be found. Note that depending on the
        /// user area, the username may be a copy of the email address.
        /// </summary>
        /// <param name="username">The username to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        public UserMicroSummary GetUserMicroSummaryByUsername(string username, string userAreaCode)
        {
            return _queryExecutor.Execute(new GetUserMicroSummaryByUsernameQuery(username, userAreaCode));
        }

        /// <summary>
        /// Finds a user with a specific username address in a specific user area 
        /// returning null if the user could not be found. Note that depending on the
        /// user area, the username may be a copy of the email address.
        /// </summary>
        /// <param name="username">The username to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        public Task<UserMicroSummary> GetUserMicroSummaryByUsernameAsync(string username, string userAreaCode)
        {
            return _queryExecutor.ExecuteAsync(new GetUserMicroSummaryByUsernameQuery(username, userAreaCode));
        }

        /// <summary>
        /// Finds a user by a database id returning a UserDetails object if it 
        /// is found, otherwise null.
        /// </summary>
        /// <param name="userId">The database id of the user to find.</param>
        public Task<UserDetails> GetUserDetailsByIdAsync(int userId)
        {
            return _queryExecutor.GetByIdAsync<UserDetails>(userId);
        }

        /// <summary>
        /// Finds a user by a database id returning a UserDetails object if it 
        /// is found, otherwise null.
        /// </summary>
        public UserMicroSummary GetUserMicroSummaryById(int userId)
        {
            return _queryExecutor.GetById<UserMicroSummary>(userId);
        }

        #endregion

        #region get current user

        /// <summary>
        /// Finds a user by a database id returning a UserDetails object if it 
        /// is found, otherwise null.
        /// </summary>
        public Task<UserMicroSummary> GetUserMicroSummaryByIdAsync(int userId)
        {
            return _queryExecutor.GetByIdAsync<UserMicroSummary>(userId);
        }

        /// <summary>
        /// Gets a UserMicroSummary object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        public UserMicroSummary GetCurrentUserMicroSummary()
        {
            return _queryExecutor.Execute(new GetCurrentUserMicroSummaryQuery());
        }

        /// <summary>
        /// Gets a UserMicroSummary object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        public Task<UserMicroSummary> GetCurrentUserMicroSummaryAsync()
        {
            return _queryExecutor.ExecuteAsync(new GetCurrentUserMicroSummaryQuery());
        }

        /// <summary>
        /// Gets a UserAccountDetails object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        public Task<UserAccountDetails> GetCurrentUserAccountDetailsAsync()
        {
            return _queryExecutor.ExecuteAsync(new GetCurrentUserAccountDetailsQuery());
        }

        #endregion

        #region searches

        /// <summary>
        /// Seaches users based on simple filter criteria and returns a paged result. 
        /// </summary>
        /// <param name="query">Search query parameters.</param>
        public Task<PagedQueryResult<UserSummary>> SearchUserSummariesAsync(SearchUserSummariesQuery query)
        {
            return _queryExecutor.ExecuteAsync(query);
        }

        #endregion

        #region utility

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <returns>True if the username is unique; otherwise false.</returns>
        public bool IsUsernameUnique(IsUsernameUniqueQuery query)
        {
            return _queryExecutor.Execute(query);
        }

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <returns>True if the username is unique; otherwise false.</returns>
        public Task<bool> IsUsernameUniqueAsync(IsUsernameUniqueQuery query)
        {
            return _queryExecutor.ExecuteAsync(query);
        }

        #endregion

        #endregion

        #region commands

        /// <summary>
        /// A generic user creation command for use with Cofoundry users and
        /// other non-Cofoundry users. Does not send any email notifications.
        /// </summary>
        public Task AddUserAsync(AddUserCommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Adds a user to the Cofoundry user area and sends a welcome notification
        /// containing a generated password which must be changed at first login.
        /// </summary>
        public Task AddCofoundryUserAsync(AddCofoundryUserCommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// A generic user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        public Task UpdateUserAsync(UpdateUserCommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Marks a user as deleted in the database (soft delete).
        /// </summary>
        /// <param name="userId">Id of the role to delete.</param>
        public Task DeleteUserAsync(int userId)
        {
            return _commandExecutor.ExecuteAsync(new DeleteUserCommand(userId));
        }

        /// <summary>
        /// Updates the user account of the currently logged in user.
        /// </summary>
        public Task UpdateCurrentUserAccountAsync(UpdateCurrentUserAccountCommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// OldPassword field to authenticate the request.
        /// </summary>
        public Task UpdateCurrentUserUserPasswordAsync(UpdateCurrentUserUserPasswordCommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }

        #endregion
    }
}
