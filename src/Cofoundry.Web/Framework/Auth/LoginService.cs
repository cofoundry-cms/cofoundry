using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
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

        public void LogAuthenticatedUserIn(int userId, bool rememberUser)
        {
            // Remove any existing login (user may be switching between login areas)
            SignOut();

            var command = new LogAuthenticatedUserInCommand() { UserId = userId };
            _commandExecutor.Execute(command);

            _userSessionService.SetCurrentUserId(userId, rememberUser);
        }

        public async Task LogAuthenticatedUserInAsync(int userId, bool rememberUser)
        {
            // Remove any existing login (user may be switching between login areas)
            SignOut();

            var command = new LogAuthenticatedUserInCommand() { UserId = userId };
            await _commandExecutor.ExecuteAsync(command);

            _userSessionService.SetCurrentUserId(userId, rememberUser);
        }

        public void SignOut()
        {
            _userSessionService.Abandon();
            _userContextService.ClearCache();
        }

        public void LogFailedLoginAttempt(string userAreaCode, string username)
        {
            var command = new LogFailedLoginAttemptCommand(userAreaCode, username);
            _commandExecutor.Execute(command);
        }

        public async Task LogFailedLoginAttemptAsync(string userAreaCode, string username)
        {
            var command = new LogFailedLoginAttemptCommand(userAreaCode, username);
            await _commandExecutor.ExecuteAsync(command);
        }
    }
}
