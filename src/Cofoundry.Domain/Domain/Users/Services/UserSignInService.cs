using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class UserSignInService : IUserSignInService
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserSessionService _userSessionService;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public UserSignInService(
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

        public virtual async Task SignInAuthenticatedUserAsync(string userAreaCode, int userId, bool rememberUser)
        {
            // Clear any existing session
            await _userSessionService.SignOutAsync(userAreaCode);

            // Log in new session
            await _userSessionService.SignInAsync(userAreaCode, userId, rememberUser);
            // Switch the ambient user area to the logged in user for the remainder of the request / scope
            await _userSessionService.SetAmbientUserAreaAsync(userAreaCode);

            // Update the user record
            var command = new LogSuccessfulAuthenticationCommand() { UserId = userId };
            await _commandExecutor.ExecuteAsync(command);

            await _messageAggregator.PublishAsync(new UserSignednMessage()
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
                await _messageAggregator.PublishAsync(new UserSignedOutMessage()
                {
                    UserId = userId.Value,
                    UserAreaCode = userAreaCode
                });
            }

            await _userSessionService.SignOutAsync(userAreaCode);
        }

        public virtual async Task SignOutAllUserAreasAsync()
        {
            var userAreas = _userAreaDefinitionRepository.GetAll();
            var messages = new List<UserSignedOutMessage>();

            foreach (var userArea in userAreas)
            {
                var userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(userArea.UserAreaCode);
                if (userId.HasValue)
                {
                    messages.Add(new UserSignedOutMessage()
                    {
                        UserAreaCode = userArea.UserAreaCode,
                        UserId = userId.Value
                    });
                }
            }

            await _userSessionService.SignOutOfAllUserAreasAsync();
            await _messageAggregator.PublishBatchAsync(messages);
        }
    }
}
