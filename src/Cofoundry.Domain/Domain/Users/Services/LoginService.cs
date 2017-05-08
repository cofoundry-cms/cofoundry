using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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
        /// <param name="userId">The id of the user to log in.</param>
        /// <param name="rememberUser">
        /// True if the user should stay logged in perminantely; false
        /// if the user should only stay logged in for the duration of
        /// the session.
        /// </param>
        public async Task LogAuthenticatedUserInAsync(int userId, bool rememberUser)
        {
            // Remove any existing login (user may be switching between login areas)
            await SignOutAsync();

            var command = new LogAuthenticatedUserInCommand() { UserId = userId };
            await _commandExecutor.ExecuteAsync(command);

            await _userSessionService.SetCurrentUserIdAsync(userId, rememberUser);
        }

        /// <summary>
        /// Logs a failed login attempt. A history of logins is used
        /// to prevent brute force login attacks.
        /// </summary>
        /// <param name="userAreaCode">The code of the user area attempting to be logged into.</param>
        /// <param name="username">The username attempting to be logged in with.</param>
        public async Task LogFailedLoginAttemptAsync(string userAreaCode, string username)
        {
            var command = new LogFailedLoginAttemptCommand(userAreaCode, username);
            await _commandExecutor.ExecuteAsync(command);
        }

        /// <summary>
        /// Signs the user out of the application and ends the session.
        /// </summary>
        public async Task SignOutAsync()
        {
            await _userSessionService.AbandonAsync();
            _userContextService.ClearCache();
        }
    }
}
