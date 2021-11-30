using System;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Mail;
using Cofoundry.Core.Data;
using Cofoundry.Domain.MailTemplates;
using Cofoundry.Core;

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
    public class InitiatePasswordResetRequestCommandHandler 
        : ICommandHandler<InitiatePasswordResetRequestCommand>
        , IIgnorePermissionCheckHandler
    {
        private const int MAX_PASSWORD_RESET_ATTEMPTS = 16;
        private const int MAX_PASSWORD_RESET_ATTEMPTS_NUMHOURS = 24;

        private readonly CofoundryDbContext _dbContext;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IClientConnectionService _clientConnectionService;
        private readonly ISecurityTokenGenerationService _securityTokenGenerationService;
        private readonly IMailService _mailService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IExecutionContextFactory _executionContextFactory;

        public InitiatePasswordResetRequestCommandHandler(
            CofoundryDbContext dbContext,
            ITransactionScopeManager transactionScopeFactory,
            IClientConnectionService clientConnectionService,
            ISecurityTokenGenerationService securityTokenGenerationService,
            IMailService mailService,
            IQueryExecutor queryExecutor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IExecutionContextFactory executionContextFactory
            )
        {
            _dbContext = dbContext;
            _transactionScopeFactory = transactionScopeFactory;
            _clientConnectionService = clientConnectionService;
            _securityTokenGenerationService = securityTokenGenerationService;
            _mailService = mailService;
            _queryExecutor = queryExecutor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _executionContextFactory = executionContextFactory;
        }

        public async Task ExecuteAsync(InitiatePasswordResetRequestCommand command, IExecutionContext executionContext)
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

            foreach (var incokpleteRequest in existingIncompleteRequests)
            {
                incokpleteRequest.IsComplete = true;
            }

            var request = CreateRequest(user, connectionInfo, executionContext);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await SendNotificationAsync(request, command, executionContext);

                await scope.CompleteAsync();
            }
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

        private Task<User> GetUserAsync(InitiatePasswordResetRequestCommand command)
        {
            var user = _dbContext
                .Users
                .FilterByUserArea(command.UserAreaCode)
                .FilterCanLogIn()
                .SingleOrDefaultAsync(u => u.Username == command.Username.Trim());

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
            InitiatePasswordResetRequestCommand command,
            IExecutionContext executionContext
            )
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(request.User.UserAreaCode);
            
            var context = await CreateMailTemplateContextAsync(request, command, executionContext);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordResetRequestedByUserTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(request.User.Email, mailTemplate);
        }

        private async Task<PasswordResetRequestedByUserTemplateBuilderContext> CreateMailTemplateContextAsync(
            UserPasswordResetRequest request, 
            InitiatePasswordResetRequestCommand command,
            IExecutionContext executionContext
            )
        {
            // user is not likely logged in so we need to elevate user 
            // privilidges here to get the user data
            var query = new GetUserSummaryByIdQuery(request.UserId);
            var adminExecutionContext = await _executionContextFactory.CreateSystemUserExecutionContextAsync(executionContext);

            var userSummary = await _queryExecutor.ExecuteAsync(query, adminExecutionContext);
            EntityNotFoundException.ThrowIfNull(userSummary, request.UserId);

            var context = new PasswordResetRequestedByUserTemplateBuilderContext()
            {
                User = userSummary,
                Token = request.Token,
                UserPasswordResetRequestId = request.UserPasswordResetRequestId,
                ResetUrlBase = command.ResetUrlBase
            };

            return context;
        }
    }
}
