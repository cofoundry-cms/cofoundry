using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Initiates a password reset request, sending a notification
    /// to the user with a url to a password reset form. This command
    /// is designed for self-service password reset so the password
    /// is not changed until the form has been completed. 
    /// </para>
    /// <para>
    /// Requests are logged and validated to prevent too many reset
    /// attempts being initiated in a set period of time.
    /// </para>
    /// </summary>
    public class InitiateUserPasswordResetRequestCommandHandler
        : ICommandHandler<InitiateUserPasswordResetRequestCommand>
        , IIgnorePermissionCheckHandler
    {
        private const int MAX_PASSWORD_RESET_ATTEMPTS = 16;
        private const int MAX_PASSWORD_RESET_ATTEMPTS_NUMHOURS = 24;

        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IClientConnectionService _clientConnectionService;
        private readonly ISecurityTokenGenerationService _securityTokenGenerationService;
        private readonly IMailService _mailService;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserDataFormatter _userDataFormatter;
        private readonly IMessageAggregator _messageAggregator;

        public InitiateUserPasswordResetRequestCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IClientConnectionService clientConnectionService,
            ISecurityTokenGenerationService securityTokenGenerationService,
            IMailService mailService,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserDataFormatter userDataFormatter,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _clientConnectionService = clientConnectionService;
            _securityTokenGenerationService = securityTokenGenerationService;
            _mailService = mailService;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userDataFormatter = userDataFormatter;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(InitiateUserPasswordResetRequestCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();
            await ValidateNumberOfAttempts(connectionInfo, executionContext);
            ValidateUserArea(command.UserAreaCode);

            var user = await GetUserAsync(command);
            if (user == null) return;

            var existingIncompleteRequests = await _dbContext
                .UserPasswordResetRequests
                .Where(r => r.UserId == user.UserId && !r.IsComplete)
                .ToListAsync();

            foreach (var incompleteRequest in existingIncompleteRequests)
            {
                incompleteRequest.IsComplete = true;
            }

            var request = CreateRequest(user, connectionInfo, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();
                await SendNotificationAsync(request, command);

                scope.QueueCompletionTask(() => OnTransactionComplete(request));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(UserPasswordResetRequest request)
        {
            await _messageAggregator.PublishAsync(new UserPasswordResetInitiatedMessage()
            {
                UserAreaCode = request.User.UserAreaCode,
                UserId = request.UserId,
                UserPasswordResetRequestId = request.UserPasswordResetRequestId
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

        private async Task ValidateNumberOfAttempts(ClientConnectionInfo connectionInfo, IExecutionContext executionContext)
        {
            var dateToDetectAttempts = executionContext.ExecutionDate.AddHours(-MAX_PASSWORD_RESET_ATTEMPTS_NUMHOURS);
            var numResetAttempts = await _dbContext
                .UserPasswordResetRequests
                .CountAsync(r => r.IPAddress == connectionInfo.IPAddress && r.CreateDate > dateToDetectAttempts);

            if (numResetAttempts > MAX_PASSWORD_RESET_ATTEMPTS)
            {
                throw new ValidationException("Maximum password reset attempts reached.");
            }
        }

        private Task<User> GetUserAsync(InitiateUserPasswordResetRequestCommand command)
        {
            var username = _userDataFormatter.UniquifyUsername(command.UserAreaCode, command.Username);
            var user = _dbContext
                .Users
                .FilterByUserArea(command.UserAreaCode)
                .FilterCanLogIn()
                .SingleOrDefaultAsync(u => u.Username == username);

            return user;
        }

        private UserPasswordResetRequest CreateRequest(
            User user,
            ClientConnectionInfo connectionInfo,
            IExecutionContext executionContext
            )
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException($"Cannot reset the password because user {user.UserId} does not have an email address.");
            }

            var request = new UserPasswordResetRequest();
            request.User = user;
            request.UserPasswordResetRequestId = Guid.NewGuid();
            request.CreateDate = executionContext.ExecutionDate;
            request.IPAddress = connectionInfo.IPAddress;
            request.Token = _securityTokenGenerationService.Generate();
            _dbContext.UserPasswordResetRequests.Add(request);

            return request;
        }

        private async Task SendNotificationAsync(
            UserPasswordResetRequest request,
            InitiateUserPasswordResetRequestCommand command
            )
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(request.User.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(request, command);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordResetRequestedByUserTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(request.User.Email, mailTemplate);
        }

        private async Task<PasswordResetRequestedByUserTemplateBuilderContext> CreateMailTemplateContextAsync(
            UserPasswordResetRequest request,
            InitiateUserPasswordResetRequestCommand command
            )
        {
            // user is not likely logged in so we need to elevate user 
            // privilidges here to get the user data
            var userSummary = await _domainRepository
                .WithElevatedPermissions()
                .ExecuteQueryAsync(new GetUserSummaryByIdQuery(request.UserId));
            EntityNotFoundException.ThrowIfNull(userSummary, request.UserId);

            // Note: Uri format will have already been validated in the command
            var baseUri = new Uri(command.ResetUrlBase, UriKind.Relative);
            var context = new PasswordResetRequestedByUserTemplateBuilderContext()
            {
                User = userSummary,
                Token = request.Token,
                UserPasswordResetRequestId = request.UserPasswordResetRequestId,
                ResetUrlBase = baseUri
            };

            return context;
        }
    }
}
