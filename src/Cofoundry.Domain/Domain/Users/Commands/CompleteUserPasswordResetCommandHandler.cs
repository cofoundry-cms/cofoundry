using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.Data;
using Cofoundry.Core.Mail;
using Cofoundry.Core;
using Cofoundry.Domain.MailTemplates;

namespace Cofoundry.Domain.Internal
{
    public class CompleteUserPasswordResetCommandHandler 
        : ICommandHandler<CompleteUserPasswordResetCommand>
        , IIgnorePermissionCheckHandler
    {
        private const int NUMHOURS_PASSWORD_RESET_VALID = 16;

        #region construstor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IMailService _mailService;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IExecutionContextFactory _executionContextFactory;

        public CompleteUserPasswordResetCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IMailService mailService,
            ITransactionScopeManager transactionScopeFactory,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IExecutionContextFactory executionContextFactory
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _mailService = mailService;
            _transactionScopeFactory = transactionScopeFactory;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _executionContextFactory = executionContextFactory;
        }

        #endregion

        public async Task ExecuteAsync(CompleteUserPasswordResetCommand command, IExecutionContext executionContext)
        {
            var validationResult = await _queryExecutor.ExecuteAsync(CreateValidationQuery(command), executionContext);
            ValidatePasswordRequest(validationResult);

            var request = await QueryPasswordRequestIfToken(command).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(request, command.UserPasswordResetRequestId);

            UpdatePasswordAndSetComplete(request, command, executionContext);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();

                if (command.SendNotification)
                {
                    await SendNotificationAsync(command, request.User, executionContext);
                }

                await scope.CompleteAsync();
            }
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

        private bool IsPasswordRecoveryDateValid(DateTime dt, IExecutionContext executionContext)
        {
            return dt > executionContext.ExecutionDate.AddHours(-NUMHOURS_PASSWORD_RESET_VALID);
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
