using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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
    public class InitiateUserAccountVerificationByEmailCommandHandler
        : ICommandHandler<InitiateUserAccountVerificationByEmailCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IMailService _mailService;
        private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;
        private readonly IMessageAggregator _messageAggregator;

        public InitiateUserAccountVerificationByEmailCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IMailService mailService,
            IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _mailService = mailService;
            _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(InitiateUserAccountVerificationByEmailCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserAndVerifyAsync(command);
            var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserArea.UserAreaCode).AccountVerification;


            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                var addAuthorizedTaskCommand = await GenerateTokenAsync(user, options, executionContext);
                await SendNotificationAsync(addAuthorizedTaskCommand, user, options);

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
                RateLimitQuantity = options.RateLimitQuantity,
                RateLimitWindow = options.RateLimitWindow,
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
            UserSummary user,
            AccountVerificationOptions options
            )
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserArea.UserAreaCode);

            var context = CreateMailTemplateContext(addAuthorizedTaskCommand.OutputToken, user, options);
            var mailTemplate = await mailTemplateBuilder.BuildAccountVerificationTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate != null)
            {
                await _mailService.SendAsync(user.Email, mailTemplate);
            }

            return context.Token;
        }

        private AccountVerificationTemplateBuilderContext CreateMailTemplateContext(
            string token,
            UserSummary user,
            AccountVerificationOptions options
            )
        {
            string url = null;

            // Can only be null if a custom IDefaultMailTemplateBuilder implementation to set the path explicitly
            if (options.VerificationUrlBase != null)
            {
                url = _authorizedTaskTokenUrlHelper.MakeUrl(options.VerificationUrlBase, token);
            }

            var context = new AccountVerificationTemplateBuilderContext()
            {
                User = user,
                Token = token,
                VerificationUrlPath = url
            };

            return context;
        }

        private async Task<UserSummary> GetUserAndVerifyAsync(InitiateUserAccountVerificationByEmailCommand command)
        {
            var user = await _domainRepository
                .WithElevatedPermissions()
                .ExecuteQueryAsync(new GetUserSummaryByIdQuery(command.UserId));
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            return user;
        }
    }
}