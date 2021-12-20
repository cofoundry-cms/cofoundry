using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class LoginService : ILoginService
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserSessionService _userSessionService;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public LoginService(
            ICommandExecutor commandExecutor,
            IUserSessionService userSessionService,
            IMessageAggregator messageAggregator,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _userSessionService = userSessionService;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public virtual async Task LogAuthenticatedUserInAsync(string userAreaCode, int userId, bool rememberUser)
        {
            // Clear any existing session
            await _userSessionService.LogUserOutAsync(userAreaCode);

            // Log in new session
            await _userSessionService.LogUserInAsync(userAreaCode, userId, rememberUser);
            // Switch the ambient user area to the logged in user for the remainder of the request / scope
            await _userSessionService.SetAmbientUserAreaAsync(userAreaCode);

            // Update the user record
            var command = new LogSuccessfulLoginCommand() { UserId = userId };
            await _commandExecutor.ExecuteAsync(command);

            await _messageAggregator.PublishAsync(new UserLoggedInMessage()
            {
                UserAreaCode = userAreaCode,
                UserId = userId
            });
        }

        public virtual async Task SignOutAsync(string userAreaCode)
        {
            var userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(userAreaCode);

            if (userId.HasValue)
            {
                await _messageAggregator.PublishAsync(new UserLoggedOutMessage()
                {
                    UserId = userId.Value,
                    UserAreaCode = userAreaCode
                });
            }

            await _userSessionService.LogUserOutAsync(userAreaCode);
        }

        public virtual async Task SignOutAllUserAreasAsync()
        {
            var userAreas = _userAreaDefinitionRepository.GetAll();
            var messages = new List<UserLoggedOutMessage>();

            foreach (var userArea in userAreas)
            {
                var userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(userArea.UserAreaCode);
                if (userId.HasValue)
                {
                    messages.Add(new UserLoggedOutMessage()
                    {
                        UserAreaCode = userArea.UserAreaCode,
                        UserId = userId.Value
                    });
                }
            }

            await _userSessionService.LogUserOutOfAllUserAreasAsync();
            await _messageAggregator.PublishBatchAsync(messages);
        }
    }
}
