using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Service for logging users in and out of the application.
    /// </summary>
    public class LoginService : ILoginService
    {
        #region Constructor
        
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserSessionService _userSessionService;
        private readonly IUserContextService _userContextService;

        public LoginService(
            ICommandExecutor commandExecutor,
            IUserSessionService userSessionService,
            IUserContextService userContextService
            )
        {
            _commandExecutor = commandExecutor;
            _userContextService = userContextService;
            _userSessionService = userSessionService;
        }

        #endregion

        /// <summary>
        /// Logs a user into the application but performs no 
        /// authentication. The user should have already passed 
        /// authentication prior to calling this method.
        /// </summary>
        /// <param name="userAreaCode">The code of the user area to log into.</param>
        /// <param name="userId">The id of the user to log in.</param>
        /// <param name="rememberUser">
        /// True if the user should stay logged in perminantely; false
        /// if the user should only stay logged in for the duration of
        /// the session.
        /// </param>
        public virtual async Task LogAuthenticatedUserInAsync(string userAreaCode, int userId, bool rememberUser)
        {
            // Clear any existing session
            await SignOutAsync(userAreaCode);

            // Log in new session
            await _userSessionService.LogUserInAsync(userAreaCode, userId, rememberUser);

            // Update the user record
            var command = new LogSuccessfulLoginCommand() { UserId = userId };
            await _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Logs a failed login attempt. A history of logins is used
        /// to prevent brute force login attacks.
        /// </summary>
        /// <param name="userAreaCode">The code of the user area attempting to be logged into.</param>
        /// <param name="username">The username attempting to be logged in with.</param>
        public virtual async Task LogFailedLoginAttemptAsync(string userAreaCode, string username)
        {
            var command = new LogFailedLoginAttemptCommand(userAreaCode, username);
            await _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Signs the user out of the application and ends the session.
        /// </summary>
        /// <param name="userAreaCode">The code of the user area to log out of.</param>
        public virtual async Task SignOutAsync(string userAreaCode)
        {
            await _userSessionService.LogUserOutAsync(userAreaCode);
            _userContextService.ClearCache();
        }

        /// <summary>
        /// Signs the user out of all user areas and ends the session.
        /// </summary>
        public virtual async Task SignOutAllUserAreasAsync()
        {
            await _userSessionService.LogUserOutOfAllUserAreasAsync();
            _userContextService.ClearCache();
        }
    }
}
