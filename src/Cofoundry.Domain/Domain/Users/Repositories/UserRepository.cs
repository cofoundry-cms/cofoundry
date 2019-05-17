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
    [Obsolete("Use the new IContentRepository instead.")]
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
        /// Finds a user by a database id returning a UserMicroSummary object if it 
        /// is found, otherwise null.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserMicroSummary> GetUserMicroSummaryByIdAsync(int userId, IExecutionContext executionContext = null)
        {
            var query = new GetUserMicroSummaryByIdQuery(userId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Finds a user with a specific email address in a specific user area 
        /// returning null if the user could not be found. Note that if the user
        /// area does not support email addresses then the email field will be empty.
        /// </summary>
        /// <param name="email">The email address to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserMicroSummary> GetUserMicroSummaryByEmailAsync(string email, string userAreaCode, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetUserMicroSummaryByEmailQuery(email, userAreaCode), executionContext);
        }

        /// <summary>
        /// Finds a user with a specific username address in a specific user area 
        /// returning null if the user could not be found. Note that depending on the
        /// user area, the username may be a copy of the email address.
        /// </summary>
        /// <param name="username">The username to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserMicroSummary> GetUserMicroSummaryByUsernameAsync(string username, string userAreaCode, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetUserMicroSummaryByUsernameQuery(username, userAreaCode), executionContext);
        }

        /// <summary>
        /// Finds a user by a database id returning a UserSummary object if it 
        /// is found, otherwise null.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserSummary> GetUserSummaryByIdAsync(int userId, IExecutionContext executionContext = null)
        {
            var query = new GetUserSummaryByIdQuery(userId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Finds a user by a database id returning a UserDetails object if it 
        /// is found, otherwise null.
        /// </summary>
        /// <param name="userId">The database id of the user to find.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserDetails> GetUserDetailsByIdAsync(int userId, IExecutionContext executionContext = null)
        {
            var query = new GetUserDetailsByIdQuery(userId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region get current user

        /// <summary>
        /// Gets a UserMicroSummary object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserMicroSummary> GetCurrentUserMicroSummaryAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetCurrentUserMicroSummaryQuery(), executionContext);
        }

        /// <summary>
        /// Gets a UserSummary object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserSummary> GetCurrentUserSummaryAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetCurrentUserSummaryQuery(), executionContext);
        }

        /// <summary>
        /// Gets a UserDetails object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserDetails> GetCurrentUserDetailsAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetCurrentUserDetailsQuery(), executionContext);
        }

        /// <summary>
        /// Gets a UserAccountDetails object representing the currently logged in 
        /// user. If the user is not logged in then null is returned.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<UserAccountDetails> GetCurrentUserAccountDetailsAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetCurrentUserAccountDetailsQuery(), executionContext);
        }

        #endregion

        #region searches

        /// <summary>
        /// Seaches users based on simple filter criteria and returns a paged result. 
        /// </summary>
        /// <param name="query">Search query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<PagedQueryResult<UserSummary>> SearchUserSummariesAsync(SearchUserSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region utility

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        /// <returns>True if the username is unique; otherwise false.</returns>
        public Task<bool> IsUsernameUniqueAsync(IsUsernameUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #endregion

        #region commands

        /// <summary>
        /// A generic user creation command for use with Cofoundry users and
        /// other non-Cofoundry users. Does not send any email notifications.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        public Task AddUserAsync(AddUserCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// Adds a user to the Cofoundry user area and sends a welcome notification
        /// containing a generated password which must be changed at first login.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        public Task AddCofoundryUserAsync(AddCofoundryUserCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// A generic user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        public Task UpdateUserAsync(UpdateUserCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// Marks a user as deleted in the database (soft delete).
        /// </summary>
        /// <param name="userId">Id of the role to delete.</param>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        public Task DeleteUserAsync(int userId, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(new DeleteUserCommand(userId), executionContext);
        }

        /// <summary>
        /// Updates the user account of the currently logged in user.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        public Task UpdateCurrentUserAccountAsync(UpdateCurrentUserAccountCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// OldPassword field to authenticate the request.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the command. Useful if you need to temporarily elevate your permission level.</param>
        public Task UpdateCurrentUserUserPasswordAsync(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        #endregion
    }
}
