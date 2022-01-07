using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Completes a password reset request initiated by
    /// <see cref="InitiateUserPasswordResetRequestCommand"/>, updating the users
    /// password if the request is verified.
    /// </summary>
    public class CompleteUserPasswordResetRequestCommandHandler
        : ICommandHandler<CompleteUserPasswordResetRequestCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IMailService _mailService;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserContextCache _userContextCache;
        private readonly IPasswordPolicyService _newPasswordValidationService;
        private readonly IMessageAggregator _messageAggregator;

        public CompleteUserPasswordResetRequestCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IMailService mailService,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _mailService = mailService;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userContextCache = userContextCache;
            _newPasswordValidationService = newPasswordValidationService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(CompleteUserPasswordResetRequestCommand command, IExecutionContext executionContext)
        {
            await ValidateRequestAsync(command, executionContext);

            var request = await GetUserPasswordResetRequestAsync(command);

            await ValidatePasswordAsync(request, command, executionContext);
            UpdatePasswordAndSetComplete(request, command, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();

                if (command.SendNotification)
                {
                    await SendNotificationAsync(command, request.User);
                }

                scope.QueueCompletionTask(() => OnTransactionComplete(request));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(UserPasswordResetRequest request)
        {
            _userContextCache.Clear(request.UserId);

            await _messageAggregator.PublishAsync(new UserPasswordResetCompletedMessage()
            {
                UserAreaCode = request.User.UserAreaCode,
                UserId = request.UserId,
                UserPasswordResetRequestId = request.UserPasswordResetRequestId
            });

            await _messageAggregator.PublishAsync(new UserPasswordUpdatedMessage()
            {
                UserAreaCode = request.User.UserAreaCode,
                UserId = request.UserId
            });
        }

        private async Task ValidateRequestAsync(
            CompleteUserPasswordResetRequestCommand command,
            IExecutionContext executionContext
            )
        {
            var query = new ValidatePasswordResetRequestQuery()
            {
                UserPasswordResetRequestId = command.UserPasswordResetRequestId,
                UserAreaCode = command.UserAreaCode,
                Token = command.Token
            };

            var result = await _domainRepository
                .WithExecutionContext(executionContext)
                .ExecuteQueryAsync(query);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Error.ToDisplayText());
            }
        }

        private async Task<UserPasswordResetRequest> GetUserPasswordResetRequestAsync(CompleteUserPasswordResetRequestCommand command)
        {
            var request = await _dbContext
                .UserPasswordResetRequests
                .Include(r => r.User)
                .Where(r => r.UserPasswordResetRequestId == command.UserPasswordResetRequestId
                    && !r.User.IsSystemAccount
                    && !r.User.IsDeleted)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(request, command.UserPasswordResetRequestId);

            return request;
        }

        private async Task ValidatePasswordAsync(UserPasswordResetRequest request, CompleteUserPasswordResetRequestCommand command, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(command.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            var context = NewPasswordValidationContext.MapFromUser(request.User);
            context.Password = command.NewPassword;
            context.PropertyName = nameof(command.NewPassword);
            context.ExecutionContext = executionContext;

            await _newPasswordValidationService.ValidateAsync(context);
        }

        private void UpdatePasswordAndSetComplete(UserPasswordResetRequest request, CompleteUserPasswordResetRequestCommand command, IExecutionContext executionContext)
        {
            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, request.User, executionContext);

            request.IsComplete = true;
        }

        private async Task SendNotificationAsync(
            CompleteUserPasswordResetRequestCommand command,
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
