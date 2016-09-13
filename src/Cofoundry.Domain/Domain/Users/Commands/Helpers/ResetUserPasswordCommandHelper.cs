using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core.Validation;
using System.Data.Entity;
using Cofoundry.Core.Mail;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper used by ResetUserPasswordByUserIdCommandHandler and ResetUserPasswordByUsernameCommandHandler
    /// for shared functionality.
    /// </summary>
    public class ResetUserPasswordCommandHelper : IResetUserPasswordCommandHelper
    {
        private const int MAX_PASSWORD_RESET_ATTEMPTS = 16;
        private const int MAX_PASSWORD_RESET_ATTEMPTS_NUMHOURS = 24;

        #region construstor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly ISecurityTokenGenerationService _securityTokenGenerationService;
        private readonly IUserAreaRepository _userAreaRepository;
        private readonly IMailService _mailService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IClientConnectionService _clientConnectionService;

        public ResetUserPasswordCommandHelper(
            CofoundryDbContext dbContext,
            IUserAreaRepository userAreaRepository,
            IPasswordCryptographyService passwordCryptographyService,
            ISecurityTokenGenerationService securityTokenGenerationService,
            IMailService mailService,
            ITransactionScopeFactory transactionScopeFactory,
            IClientConnectionService clientConnectionService
            )
        {
            _dbContext = dbContext;
            _userAreaRepository = userAreaRepository;
            _passwordCryptographyService = passwordCryptographyService;
            _securityTokenGenerationService = securityTokenGenerationService;
            _mailService = mailService;
            _transactionScopeFactory = transactionScopeFactory;
            _clientConnectionService = clientConnectionService;
        }

        #endregion

        #region public methods

        public void ValidateCommand(IResetUserPasswordCommand command, IExecutionContext executionContext)
        {
            var numResetAttempts = QueryNumberResetAttempts(executionContext).Count();
            ValidateNumberOfResetAttempts(numResetAttempts);
        }

        public async Task ValidateCommandAsync(IResetUserPasswordCommand command, IExecutionContext executionContext)
        {
            var numResetAttempts = await QueryNumberResetAttempts(executionContext).CountAsync();
            ValidateNumberOfResetAttempts(numResetAttempts);
        }

        public void ResetPassword(User user, IResetUserPasswordCommand command, IExecutionContext executionContext)
        {
            ValidateUserAccountExists(user);
            ValidateUserArea(user.UserAreaCode);

            var existingIncompleteRequests = QueryIncompleteRequests(user).ToList();
            SetExistingRequestsComplete(existingIncompleteRequests);
            var request = CreateRequest(executionContext, user);
            SetMailTemplate(command, user, request);

            using (var scope = _transactionScopeFactory.Create())
            {
                _dbContext.SaveChanges();
                _mailService.Send(user.Email, user.GetFullName(), command.MailTemplate);

                scope.Complete();
            }
        }

        public async Task ResetPasswordAsync(User user, IResetUserPasswordCommand command, IExecutionContext executionContext)
        {
            ValidateUserAccountExists(user);
            ValidateUserArea(user.UserAreaCode);

            var existingIncompleteRequests = await QueryIncompleteRequests(user).ToListAsync();
            SetExistingRequestsComplete(existingIncompleteRequests);
            var request = CreateRequest(executionContext, user);
            SetMailTemplate(command, user, request);

            using (var scope = _transactionScopeFactory.Create())
            {
                await _dbContext.SaveChangesAsync();
                await _mailService.SendAsync(user.Email, user.GetFullName(), command.MailTemplate);

                scope.Complete();
            }
        }

        #endregion

        #region private helpers

        private void ValidateUserArea(string userAreaCode)
        {
            var userArea = _userAreaRepository.GetByCode(userAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException("Cannot reset the password because the " + userArea.Name + " user area does not allow password logins.");
            }
        }

        private IQueryable<UserPasswordResetRequest> QueryNumberResetAttempts(IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            var dateToDetectAttempts = executionContext.ExecutionDate.AddHours(-MAX_PASSWORD_RESET_ATTEMPTS_NUMHOURS);
            return _dbContext.UserPasswordResetRequests.Where(r => r.IPAddress == connectionInfo.IPAddress && r.CreateDate > dateToDetectAttempts);
        }

        private static void ValidateNumberOfResetAttempts(int numResetAttempts)
        {
            if (numResetAttempts > MAX_PASSWORD_RESET_ATTEMPTS)
            {
                throw new ValidationException("Maximum password reset attempts reached.");
            }
        }

        private static void ValidateUserAccountExists(User user)
        {
            if (user == null)
            {
                throw new PropertyValidationException("Account not found.", "Username");
            }
        }

        private IQueryable<UserPasswordResetRequest> QueryIncompleteRequests(User user)
        {
            return _dbContext
                .UserPasswordResetRequests
                .Where(r => r.UserId == user.UserId && !r.IsComplete);
        }

        private void SetExistingRequestsComplete(List<UserPasswordResetRequest> existingIncompleteRequests)
        {
            foreach (var req in existingIncompleteRequests)
            {
                req.IsComplete = true;
            }
        }

        private UserPasswordResetRequest CreateRequest(IExecutionContext executionContext, User user)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            var request = new UserPasswordResetRequest();
            request.User = user;
            request.UserPasswordResetRequestId = Guid.NewGuid();
            request.CreateDate = executionContext.ExecutionDate;
            request.IPAddress = connectionInfo.IPAddress;
            request.Token = _securityTokenGenerationService.Generate();
            _dbContext.UserPasswordResetRequests.Add(request);

            return request;
        }

        private void SetMailTemplate(IResetUserPasswordCommand command, User user, UserPasswordResetRequest request)
        {
            command.MailTemplate.FirstName = user.FirstName;
            command.MailTemplate.LastName = user.LastName;
            command.MailTemplate.UserPasswordResetRequestId = request.UserPasswordResetRequestId;
            command.MailTemplate.Token = request.Token;
        }

        #endregion
    }
}
