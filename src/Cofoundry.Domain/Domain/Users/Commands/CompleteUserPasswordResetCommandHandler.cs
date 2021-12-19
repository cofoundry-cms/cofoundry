using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class CompleteUserPasswordResetCommandHandler
        : ICommandHandler<CompleteUserPasswordResetCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IMailService _mailService;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IUserContextCache _userContextCache;
        private readonly IPasswordPolicyService _newPasswordValidationService;

        public CompleteUserPasswordResetCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IMailService mailService,
            ITransactionScopeManager transactionScopeFactory,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IExecutionContextFactory transactionScopeManager,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _mailService = mailService;
            _transactionScopeManager = transactionScopeFactory;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _executionContextFactory = transactionScopeManager;
            _userContextCache = userContextCache;
            _newPasswordValidationService = newPasswordValidationService;
        }

        public async Task ExecuteAsync(CompleteUserPasswordResetCommand command, IExecutionContext executionContext)
        {
            var validationResult = await _queryExecutor.ExecuteAsync(CreateValidationQuery(command), executionContext);
            ValidatePasswordRequest(validationResult);

            var request = await QueryPasswordRequestIfToken(command).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(request, command.UserPasswordResetRequestId);

            await ValidatePasswordAsync(request, command);
            UpdatePasswordAndSetComplete(request, command, executionContext);

            using (var scope = _transactionScopeManager.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();

                if (command.SendNotification)
                {
                    await SendNotificationAsync(command, request.User, executionContext);
                }

                scope.QueueCompletionTask(() => _userContextCache.Clear(request.UserId));
                await scope.CompleteAsync();
            }
        }

        private async Task ValidatePasswordAsync(UserPasswordResetRequest request, CompleteUserPasswordResetCommand command)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(command.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            var context = NewPasswordValidationContext.MapFromUser(request.User);
            context.Password = command.NewPassword;
            context.PropertyName = nameof(command.NewPassword);

            await _newPasswordValidationService.ValidateAsync(context);
        }

        private void UpdatePasswordAndSetComplete(UserPasswordResetRequest request, CompleteUserPasswordResetCommand command, IExecutionContext executionContext)
        {
            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, request.User, executionContext);

            request.IsComplete = true;
        }

        private IQueryable<UserPasswordResetRequest> QueryPasswordRequestIfToken(CompleteUserPasswordResetCommand command)
        {
            return _dbContext
                .UserPasswordResetRequests
                .Include(r => r.User)
                .Where(r => r.UserPasswordResetRequestId == command.UserPasswordResetRequestId
                    && !r.User.IsSystemAccount
                    && !r.User.IsDeleted);
        }

        private ValidatePasswordResetRequestQuery CreateValidationQuery(CompleteUserPasswordResetCommand command)
        {
            var query = new ValidatePasswordResetRequestQuery();
            query.UserPasswordResetRequestId = command.UserPasswordResetRequestId;
            query.UserAreaCode = command.UserAreaCode;
            query.Token = command.Token;

            return query;
        }

        private void ValidatePasswordRequest(PasswordResetRequestAuthenticationResult result)
        {
            if (!result.IsValid)
            {
                throw new ValidationException(result.Error.ToDisplayText());
            }
        }

        private async Task SendNotificationAsync(
            CompleteUserPasswordResetCommand command,
            User user,
            IExecutionContext executionContext
            )
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(command.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(user.UserId, executionContext);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(user.Email, mailTemplate);
        }

        private async Task<PasswordChangedTemplateBuilderContext> CreateMailTemplateContextAsync(
            int userId,
            IExecutionContext executionContext
            )
        {
            // user is will not be logged in so we need to elevate user 
            // privilidges here to get the user data
            var query = new GetUserSummaryByIdQuery(userId);
            var adminExecutionContext = await _executionContextFactory.CreateSystemUserExecutionContextAsync(executionContext);

            var userSummary = await _queryExecutor.ExecuteAsync(query, adminExecutionContext);
            EntityNotFoundException.ThrowIfNull(userSummary, userId);

            var context = new PasswordChangedTemplateBuilderContext()
            {
                User = userSummary
            };

            return context;
        }
    }
}
