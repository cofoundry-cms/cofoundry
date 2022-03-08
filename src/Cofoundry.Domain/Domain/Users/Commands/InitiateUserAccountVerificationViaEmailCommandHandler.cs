using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// <para>
    /// Initiates an email-based account verification (AKA "confirm account") request, sending an 
    /// email notification to the user with a url to an account verification page. 
    /// </para>
    /// <para>
    /// This command utilises the "Authorized Task" framework to do most of the work, and if
    /// you want a more customized process then you may wish to use the authorized task framework 
    /// directly.
    /// </para>
    /// </summary>
    public class InitiateUserAccountVerificationViaEmailCommandHandler
        : ICommandHandler<InitiateUserAccountVerificationViaEmailCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserSummaryMapper _userSummaryMapper;
        private readonly IUserMailTemplateBuilderContextFactory _userMailTemplateBuilderContextFactory;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IMailService _mailService;
        private readonly IMessageAggregator _messageAggregator;

        public InitiateUserAccountVerificationViaEmailCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserSummaryMapper userSummaryMapper,
            IUserMailTemplateBuilderContextFactory userMailTemplateBuilderContextFactory,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IMailService mailService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userSummaryMapper = userSummaryMapper;
            _userMailTemplateBuilderContextFactory = userMailTemplateBuilderContextFactory;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _mailService = mailService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(InitiateUserAccountVerificationViaEmailCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserAndVerifyAsync(command);
            var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserArea.UserAreaCode).AccountVerification;

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                var addAuthorizedTaskCommand = await GenerateTokenAsync(user, options, executionContext);
                await SendNotificationAsync(addAuthorizedTaskCommand, user);

                scope.QueueCompletionTask(() => OnTransactionComplete(user, addAuthorizedTaskCommand));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(UserSummary user, AddAuthorizedTaskCommand addAuthorizedTaskCommand)
        {
            await _messageAggregator.PublishAsync(new UserAccountVerificationInitiatedMessage()
            {
                AuthorizedTaskId = addAuthorizedTaskCommand.OutputAuthorizedTaskId,
                UserAreaCode = user.UserArea.UserAreaCode,
                UserId = user.UserId,
                Token = addAuthorizedTaskCommand.OutputToken
            });
        }

        private async Task<AddAuthorizedTaskCommand> GenerateTokenAsync(
            UserSummary user,
            AccountVerificationOptions options,
            IExecutionContext executionContext
            )
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException($"Cannot verify the user account because user {user.UserId} does not have an email address.");
            }

            var command = new AddAuthorizedTaskCommand()
            {
                AuthorizedTaskTypeCode = UserAccountVerificationAuthorizedTaskType.Code,
                UserId = user.UserId,
                RateLimit = options.InitiationRateLimit,
                ExpireAfter = options.ExpireAfter,
                TaskData = user.Email
            };

            try
            {
                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(command);
            }
            catch (ValidationErrorException ex)
            {
                if (ex.ErrorCode == AuthorizedTaskValidationErrors.Create.RateLimitExceeded.ErrorCode)
                {
                    // Use a more specific error message
                    UserValidationErrors.AccountVerification.Initiation.RateLimitExceeded.Throw();
                }

                throw;
            }

            return command;
        }

        private async Task<string> SendNotificationAsync(
            AddAuthorizedTaskCommand addAuthorizedTaskCommand,
            UserSummary user
            )
        {
            var context = _userMailTemplateBuilderContextFactory.CreateAccountVerificationContext(user, addAuthorizedTaskCommand.OutputToken);
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserArea.UserAreaCode);
            var mailTemplate = await mailTemplateBuilder.BuildAccountVerificationTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate != null)
            {
                await _mailService.SendAsync(user.Email, mailTemplate);
            }

            return context.Token;
        }

        private async Task<UserSummary> GetUserAndVerifyAsync(InitiateUserAccountVerificationViaEmailCommand command)
        {
            var dbUser = await _dbContext
                .Users
                .AsNoTracking()
                .IncludeForSummary()
                .FilterCanSignIn()
                .FilterById(command.UserId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(dbUser, command.UserId);

            if (dbUser.AccountVerifiedDate.HasValue)
            {
                UserValidationErrors.AccountVerification.Initiation.AlreadyVerified.Throw();
            }

            var user = _userSummaryMapper.Map(dbUser);

            return user;
        }
    }
}