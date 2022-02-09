using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Signs the user out of all user areas and ends the session. A
    /// <see cref="UserSignedOutMessage"/> is published for each user
    /// area that is logged out.
    /// </summary>
    public class SignOutCurrentUserFromAllUserAreasCommandHandler
        : ICommandHandler<SignOutCurrentUserFromAllUserAreasCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserSessionService _userSessionService;
        private readonly IMessageAggregator _messageAggregator;

        public SignOutCurrentUserFromAllUserAreasCommandHandler(
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserSessionService userSessionService,
            IMessageAggregator messageAggregator
            )
        {
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userSessionService = userSessionService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(SignOutCurrentUserFromAllUserAreasCommand command, IExecutionContext executionContext)
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

            await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(messages));
        }

        private async Task OnTransactionComplete(List<UserSignedOutMessage> messages)
        {
            await _messageAggregator.PublishBatchAsync(messages);
        }
    }
}