using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Signs in a user that has already passed an authentication check. The user 
    /// should have already passed authentication prior to calling this method. The 
    /// ambient user area (i.e. "current" user context) is switched to the specified area 
    /// for the remainder of the DI scope (i.e. request for web apps).
    /// </summary>
    public class SignInAuthenticatedUserCommandHandler
        : ICommandHandler<SignInAuthenticatedUserCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserSessionService _userSessionService;
        private readonly IMessageAggregator _messageAggregator;

        public SignInAuthenticatedUserCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserSessionService userSessionService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userSessionService = userSessionService;
            _messageAggregator = messageAggregator;
        }


        public async Task ExecuteAsync(SignInAuthenticatedUserCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserInfoAsync(command);
            ValidateUserCanLogin(user);
            SetLoggedIn(user, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();

                // Successful credentials auth invalidates any account recovery requests
                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(new InvalidateAuthorizedTaskBatchCommand(command.UserId, UserAccountRecoveryAuthorizedTaskType.Code));

                await SignInAuthenticatedUserAsync(
                    user.UserAreaCode,
                    command.UserId,
                    command.RememberUser
                    );

                scope.QueueCompletionTask(() => OnTransactionComplete(command, user));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(SignInAuthenticatedUserCommand command, User user)
        {
            await _messageAggregator.PublishAsync(new UserSignednMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = command.UserId
            });
        }

        private async Task<User> GetUserInfoAsync(SignInAuthenticatedUserCommand command)
        {
            var user = await _dbContext
                .Users
                .FilterCanSignIn()
                .FilterById(command.UserId)
                .FirstOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            return user;
        }

        private void ValidateUserCanLogin(User user)
        {
            if (user.RequirePasswordChange)
            {
                UserValidationErrors.Authentication.PasswordChangeRequired.Throw();
            }

            if (!user.AccountVerifiedDate.HasValue)
            {
                var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserAreaCode);

                if (options.AccountVerification.RequireVerification)
                {
                    UserValidationErrors.Authentication.AccountNotVerified.Throw();
                }
            }
        }

        private void SetLoggedIn(User user, IExecutionContext executionContext)
        {
            user.PreviousSignInDate = user.LastSignInDate;
            user.LastSignInDate = executionContext.ExecutionDate;
        }

        private async Task SignInAuthenticatedUserAsync(string userAreaCode, int userId, bool rememberUser)
        {
            // Clear any existing session
            await _userSessionService.SignOutAsync(userAreaCode);

            // Log in new session
            await _userSessionService.SignInAsync(userAreaCode, userId, rememberUser);

            // Switch the ambient user area to the logged in user for the remainder of the request / scope
            await _userSessionService.SetAmbientUserAreaAsync(userAreaCode);
        }
    }
}