using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Completes an account recovery request initiated by
    /// <see cref="InitiateUserAccountRecoveryViaEmailCommand"/>, updating the users
    /// password if the request is verified.
    /// </summary>
    public class CompleteUserAccountRecoveryViaEmailCommandHandler
        : ICommandHandler<CompleteUserAccountRecoveryViaEmailCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
        private readonly IUserContextCache _userContextCache;
        private readonly IPasswordPolicyService _newPasswordValidationService;
        private readonly IMessageAggregator _messageAggregator;

        public CompleteUserAccountRecoveryViaEmailCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
            _userContextCache = userContextCache;
            _newPasswordValidationService = newPasswordValidationService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(CompleteUserAccountRecoveryViaEmailCommand command, IExecutionContext executionContext)
        {
            var validationResult = await ValidateRequestAsync(command, executionContext);
            var user = await GetUserAsync(validationResult.Data.UserId);

            await ValidatePasswordAsync(user, command, executionContext);
            UpdatePassword(user, command, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();

                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(new CompleteAuthorizedTaskCommand()
                    {
                        AuthorizedTaskId = validationResult.Data.AuthorizedTaskId
                    });

                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(new InvalidateAuthorizedTaskBatchCommand(user.UserId, UserAccountRecoveryAuthorizedTaskType.Code));

                await _passwordUpdateCommandHelper.SendPasswordChangedNotification(user);

                scope.QueueCompletionTask(() => OnTransactionComplete(user, validationResult));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(User user, AuthorizedTaskTokenValidationResult validationResult)
        {
            _userContextCache.Clear(user.UserId);

            await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);

            await _messageAggregator.PublishAsync(new UserAccountRecoveryCompletedMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId,
                AuthorizedTaskId = validationResult.Data.AuthorizedTaskId
            });

            await _messageAggregator.PublishAsync(new UserPasswordUpdatedMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId
            });
        }

        private async Task<AuthorizedTaskTokenValidationResult> ValidateRequestAsync(
            CompleteUserAccountRecoveryViaEmailCommand command,
            IExecutionContext executionContext
            )
        {
            var query = new ValidateUserAccountRecoveryByEmailQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Token = command.Token
            };

            var result = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(query);
            result.ThrowIfNotSuccess();

            return result;
        }

        private async Task<User> GetUserAsync(int userId)
        {
            var user = await _dbContext
                .Users
                .IncludeForSummary()
                .FilterEnabled()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, userId);

            return user;
        }

        private async Task ValidatePasswordAsync(User user, CompleteUserAccountRecoveryViaEmailCommand command, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(command.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            var context = NewPasswordValidationContext.MapFromUser(user);
            context.Password = command.NewPassword;
            context.PropertyName = nameof(command.NewPassword);
            context.ExecutionContext = executionContext;

            await _newPasswordValidationService.ValidateAsync(context);
        }

        private void UpdatePassword(User user, CompleteUserAccountRecoveryViaEmailCommand command, IExecutionContext executionContext)
        {
            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);
            _userSecurityStampUpdateHelper.Update(user);
        }
    }
}