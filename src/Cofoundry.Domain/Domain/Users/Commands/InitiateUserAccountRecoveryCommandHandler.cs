using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Initiates an account recovery (AKA "forgot password") request, sending a 
    /// notification to the user with a url to an account recovery form. This command
    /// is designed for self-service password reset so the password is not changed 
    /// until the form has been completed. 
    /// </para>
    /// <para>
    /// Requests are logged and validated to prevent too many account recovery
    /// attempts being initiated in a set period of time.
    /// </para>
    /// </summary>
    public class InitiateUserAccountRecoveryCommandHandler
        : ICommandHandler<InitiateUserAccountRecoveryCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IClientConnectionService _clientConnectionService;
        private readonly IRandomStringGenerator _randomStringGenerator;
        private readonly IMailService _mailService;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserDataFormatter _userDataFormatter;
        private readonly IUserAccountRecoveryTokenFormatter _userAccountRecoveryTokenFormatter;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IUserAccountRecoveryUrlHelper _userAccountRecoveryUrlHelper;

        public InitiateUserAccountRecoveryCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IClientConnectionService clientConnectionService,
            IRandomStringGenerator randomStringGenerator,
            IMailService mailService,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserDataFormatter userDataFormatter,
            IUserAccountRecoveryTokenFormatter userAccountRecoveryTokenFormatter,
            IMessageAggregator messageAggregator,
            IUserAccountRecoveryUrlHelper userAccountRecoveryUrlHelper
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _clientConnectionService = clientConnectionService;
            _randomStringGenerator = randomStringGenerator;
            _mailService = mailService;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userDataFormatter = userDataFormatter;
            _userAccountRecoveryTokenFormatter = userAccountRecoveryTokenFormatter;
            _messageAggregator = messageAggregator;
            _userAccountRecoveryUrlHelper = userAccountRecoveryUrlHelper;
        }

        public async Task ExecuteAsync(InitiateUserAccountRecoveryCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();
            ValidateUserArea(command.UserAreaCode);
            var options = _userAreaDefinitionRepository.GetOptionsByCode(command.UserAreaCode).AccountRecovery;
            await ValidateNumberOfAttempts(options, connectionInfo, executionContext);

            var user = await GetUserAsync(command);
            if (user == null) return;

            var request = CreateRequest(user, connectionInfo, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();
                var token = await SendNotificationAsync(request, command, options);

                scope.QueueCompletionTask(() => OnTransactionComplete(request, token));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(UserAccountRecoveryRequest request, string token)
        {
            await _messageAggregator.PublishAsync(new UserAccountRecoveryInitiatedMessage()
            {
                UserAreaCode = request.User.UserAreaCode,
                UserId = request.UserId,
                UserAccountRecoveryRequestId = request.UserAccountRecoveryRequestId,
                Token = token
            });
        }

        private void ValidateUserArea(string userAreaCode)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not allow password logins.");
            }

            if (!userArea.UseEmailAsUsername)
            {
                throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not require email addresses.");
            }
        }

        private async Task ValidateNumberOfAttempts(
            AccountRecoveryOptions options,
            ClientConnectionInfo connectionInfo,
            IExecutionContext executionContext
            )
        {
            if (options.MaxAttemptsWindow <= TimeSpan.Zero || options.MaxAttempts < 1) return;

            var dateToDetectAttempts = executionContext.ExecutionDate.Add(-options.MaxAttemptsWindow);
            var numResetAttempts = await _dbContext
                .UserAccountRecoveryRequests
                .CountAsync(r => r.IPAddress == connectionInfo.IPAddress && r.CreateDate > dateToDetectAttempts && r.CreateDate <= executionContext.ExecutionDate);

            if (numResetAttempts >= options.MaxAttempts)
            {
                UserValidationErrors.AccountRecovery.Initiation.MaxAttemptsExceeded.Throw();
            }
        }

        private Task<User> GetUserAsync(InitiateUserAccountRecoveryCommand command)
        {
            var username = _userDataFormatter.UniquifyUsername(command.UserAreaCode, command.Username);
            var user = _dbContext
                .Users
                .FilterByUserArea(command.UserAreaCode)
                .FilterCanLogIn()
                .SingleOrDefaultAsync(u => u.Username == username);

            return user;
        }

        private UserAccountRecoveryRequest CreateRequest(
            User user,
            ClientConnectionInfo connectionInfo,
            IExecutionContext executionContext
            )
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException($"Cannot reset the password because user {user.UserId} does not have an email address.");
            }

            var request = new UserAccountRecoveryRequest();
            request.User = user;
            request.UserAccountRecoveryRequestId = Guid.NewGuid();
            request.CreateDate = executionContext.ExecutionDate;
            request.IPAddress = connectionInfo.IPAddress;
            request.AuthorizationCode = _randomStringGenerator.Generate(32);
            _dbContext.UserAccountRecoveryRequests.Add(request);

            return request;
        }

        private async Task<string> SendNotificationAsync(
            UserAccountRecoveryRequest request,
            InitiateUserAccountRecoveryCommand command,
            AccountRecoveryOptions options
            )
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(request.User.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(request, command, options);
            var mailTemplate = await mailTemplateBuilder.BuildAccountRecoveryTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate != null)
            {
                await _mailService.SendAsync(request.User.Email, mailTemplate);
            }

            return context.Token;
        }

        private async Task<AccountRecoveryTemplateBuilderContext> CreateMailTemplateContextAsync(
            UserAccountRecoveryRequest request,
            InitiateUserAccountRecoveryCommand command,
            AccountRecoveryOptions options
            )
        {
            // user is not likely logged in so we need to elevate user 
            // privilidges here to get the user data
            var userSummary = await _domainRepository
                .WithElevatedPermissions()
                .ExecuteQueryAsync(new GetUserSummaryByIdQuery(request.UserId));
            EntityNotFoundException.ThrowIfNull(userSummary, request.UserId);

            var token = _userAccountRecoveryTokenFormatter.Format(new UserAccountRecoveryTokenParts()
            {
                UserAccountRecoveryRequestId = request.UserAccountRecoveryRequestId,
                AuthorizationCode = request.AuthorizationCode
            });

            string url = null;

            // Can only be null if a custom IDefaultMailTemplateBuilder implementation to set the path explicitly
            if (options.RecoveryUrlBase != null)
            {
                url = _userAccountRecoveryUrlHelper.MakeUrl(options.RecoveryUrlBase, token);
            }

            var context = new AccountRecoveryTemplateBuilderContext()
            {
                User = userSummary,
                Token = token,
                RecoveryUrlPath = url
            };

            return context;
        }
    }
}
