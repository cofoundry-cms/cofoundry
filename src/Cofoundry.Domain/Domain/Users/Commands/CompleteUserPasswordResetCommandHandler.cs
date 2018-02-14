using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core.Mail;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class CompleteUserPasswordResetCommandHandler 
        : IAsyncCommandHandler<CompleteUserPasswordResetCommand>
        , IIgnorePermissionCheckHandler
    {
        private const int NUMHOURS_PASSWORD_RESET_VALID = 16;

        #region construstor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IResetUserPasswordCommandHelper _resetUserPasswordCommandHelper;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IMailService _mailService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public CompleteUserPasswordResetCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPasswordCryptographyService passwordCryptographyService,
            IResetUserPasswordCommandHelper resetUserPasswordCommandHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IMailService mailService,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _passwordCryptographyService = passwordCryptographyService;
            _resetUserPasswordCommandHelper = resetUserPasswordCommandHelper;
            _userAreaRepository = userAreaRepository;
            _mailService = mailService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(CompleteUserPasswordResetCommand command, IExecutionContext executionContext)
        {
            var validationResult = await _queryExecutor.ExecuteAsync(CreateValidationQuery(command));
            ValidatePasswordRequest(validationResult);

            var request = await QueryPasswordRequestIfToken(command).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(request, command.UserPasswordResetRequestId);

            UpdatePasswordAndSetComplete(request, command, executionContext);
            SetMailTemplate(command, request.User);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _mailService.SendAsync(request.User.Email, request.User.GetFullName(), command.MailTemplate);

                scope.Complete();
            }
        }

        #endregion

        #region private helpers

        private void UpdatePasswordAndSetComplete(UserPasswordResetRequest request, CompleteUserPasswordResetCommand command, IExecutionContext executionContext)
        {
            var user = request.User;

            user.RequirePasswordChange = false;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;

            var hashResult = _passwordCryptographyService.CreateHash(command.NewPassword);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
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
                throw new ValidationException(result.ValidationErrorMessage);
            }
        }

        private bool IsPasswordRecoveryDateValid(DateTime dt, IExecutionContext executionContext)
        {
            return dt > executionContext.ExecutionDate.AddHours(-NUMHOURS_PASSWORD_RESET_VALID);
        }

        private void SetMailTemplate(CompleteUserPasswordResetCommand command, User user)
        {
            command.MailTemplate.FirstName = user.FirstName;
            command.MailTemplate.LastName = user.LastName;
        }

        #endregion
    }
}
