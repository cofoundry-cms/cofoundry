using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Cofoundry.Domain.MailTemplates;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Completes an account recovery request initiated by
    /// <see cref="InitiateUserAccountRecoveryCommand"/>, updating the users
    /// password if the request is verified.
    /// </summary>
    public class CompleteUserAccountRecoveryCommandHandler
        : ICommandHandler<CompleteUserAccountRecoveryCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly IMailService _mailService;
        private readonly IUserAccountRecoveryTokenFormatter _userAccountRecoveryTokenFormatter;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserContextCache _userContextCache;
        private readonly IPasswordPolicyService _newPasswordValidationService;
        private readonly IMessageAggregator _messageAggregator;

        public CompleteUserAccountRecoveryCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserStoredProcedures userStoredProcedures,
            IMailService mailService,
            IUserAccountRecoveryTokenFormatter userAccountRecoveryTokenFormatter,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userStoredProcedures = userStoredProcedures;
            _mailService = mailService;
            _userAccountRecoveryTokenFormatter = userAccountRecoveryTokenFormatter;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userContextCache = userContextCache;
            _newPasswordValidationService = newPasswordValidationService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(CompleteUserAccountRecoveryCommand command, IExecutionContext executionContext)
        {
            var tokenParts = await ValidateRequestAsync(command, executionContext);
            var request = await GetUserAccountRecoveryRequestAsync(tokenParts);

            await ValidatePasswordAsync(request, command, executionContext);
            UpdatePasswordAndSetComplete(request, command, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();
                await _userStoredProcedures.InvalidateUserAccountRecoveryRequests(request.User.UserId, executionContext.ExecutionDate);

                if (command.SendNotification)
                {
                    await SendNotificationAsync(command, request.User);
                }

                scope.QueueCompletionTask(() => OnTransactionComplete(request));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(UserAccountRecoveryRequest request)
        {
            _userContextCache.Clear(request.UserId);

            await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(request.User);

            await _messageAggregator.PublishAsync(new UserAccountRecoveryCompletedMessage()
            {
                UserAreaCode = request.User.UserAreaCode,
                UserId = request.UserId,
                UserAccountRecoveryRequestId = request.UserAccountRecoveryRequestId
            });

            await _messageAggregator.PublishAsync(new UserPasswordUpdatedMessage()
            {
                UserAreaCode = request.User.UserAreaCode,
                UserId = request.UserId
            });
        }

        private async Task<UserAccountRecoveryTokenParts> ValidateRequestAsync(
            CompleteUserAccountRecoveryCommand command,
            IExecutionContext executionContext
            )
        {
            var query = new ValidateUserAccountRecoveryRequestQuery()
            {
                Token = command.Token,
                UserAreaCode = command.UserAreaCode
            };

            var result = await _domainRepository
                .WithExecutionContext(executionContext)
                .ExecuteQueryAsync(query);

            if (!result.IsValid)
            {
                var error = result.Errors.First();
                throw new ValidationErrorException(error);
            }

            var parts = _userAccountRecoveryTokenFormatter.Parse(command.Token);
            if (parts == null)
            {
                throw new EntityInvalidOperationException($"{nameof(ValidateUserAccountRecoveryRequestQuery)} returned a valid result, but token parts could not be parsed.");
            }

            return parts;
        }

        private async Task<UserAccountRecoveryRequest> GetUserAccountRecoveryRequestAsync(UserAccountRecoveryTokenParts tokenParts)
        {
            var request = await _dbContext
                .UserAccountRecoveryRequests
                .Include(r => r.User)
                .Where(r => r.UserAccountRecoveryRequestId == tokenParts.UserAccountRecoveryRequestId
                    && !r.User.IsSystemAccount
                    && !r.User.IsDeleted)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(request, tokenParts.UserAccountRecoveryRequestId);

            return request;
        }

        private async Task ValidatePasswordAsync(UserAccountRecoveryRequest request, CompleteUserAccountRecoveryCommand command, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(command.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            var context = NewPasswordValidationContext.MapFromUser(request.User);
            context.Password = command.NewPassword;
            context.PropertyName = nameof(command.NewPassword);
            context.ExecutionContext = executionContext;

            await _newPasswordValidationService.ValidateAsync(context);
        }

        private void UpdatePasswordAndSetComplete(UserAccountRecoveryRequest request, CompleteUserAccountRecoveryCommand command, IExecutionContext executionContext)
        {
            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, request.User, executionContext);

            _userSecurityStampUpdateHelper.Update(request.User);
            request.CompletedDate = executionContext.ExecutionDate;
        }

        private async Task SendNotificationAsync(
            CompleteUserAccountRecoveryCommand command,
            User user
            )
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(command.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(user.UserId);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(user.Email, mailTemplate);
        }

        private async Task<PasswordChangedTemplateBuilderContext> CreateMailTemplateContextAsync(int userId)
        {
            // user is will not be logged in so we need to elevate user 
            // privilidges here to get the user data
            var userSummary = await _domainRepository
                .WithElevatedPermissions()
                .ExecuteQueryAsync(new GetUserSummaryByIdQuery(userId));
            EntityNotFoundException.ThrowIfNull(userSummary, userId);

            var context = new PasswordChangedTemplateBuilderContext()
            {
                User = userSummary
            };

            return context;
        }
    }
}
