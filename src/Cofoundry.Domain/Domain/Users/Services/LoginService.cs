using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class LoginService : ILoginService
    {
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

        public virtual async Task LogAuthenticatedUserInAsync(string userAreaCode, int userId, bool rememberUser)
        {
            // Clear any existing session
            await SignOutAsync(userAreaCode);

            // Log in new session
            await _userSessionService.LogUserInAsync(userAreaCode, userId, rememberUser);
            // Switch the ambient user area to the logged in user for the remainder of the request / scope
            await _userSessionService.SetAmbientUserAreaAsync(userAreaCode);

            // Update the user record
            var command = new LogSuccessfulLoginCommand() { UserId = userId };
            await _commandExecutor.ExecuteAsync(command);
        }

        public virtual async Task SignOutAsync(string userAreaCode)
        {
            await _userSessionService.LogUserOutAsync(userAreaCode);
        }

        public virtual async Task SignOutAllUserAreasAsync()
        {
            await _userSessionService.LogUserOutOfAllUserAreasAsync();
        }
    }
}
