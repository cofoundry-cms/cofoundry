using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.MailTemplates;
using Cofoundry.Core.Mail;
using Cofoundry.Core.Data;
using Cofoundry.Core;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Resets a users password to a randomly generated temporary value
    /// and sends it in a mail a notification to the user. The password
    /// will need to be changed at first login (if the user area supports 
    /// it). This is designed to be used from an admin screen rather than 
    /// a self-service reset which can be done via 
    /// InitiatePasswordResetRequestCommand.
    /// </summary>
    public class ResetUserPasswordCommandHandler 
        : IAsyncCommandHandler<ResetUserPasswordCommand>
        , IIgnorePermissionCheckHandler
    {
        #region construstor

        private readonly CofoundryDbContext _dbContext;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IMailService _mailService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IPasswordGenerationService _passwordGenerationService;

        public ResetUserPasswordCommandHandler(
            CofoundryDbContext dbContext,
            ITransactionScopeManager transactionScopeFactory,
            IMailService mailService,
            IQueryExecutor queryExecutor,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPasswordCryptographyService passwordCryptographyService,
            IPasswordGenerationService passwordGenerationService
            )
        {
            _dbContext = dbContext;
            _transactionScopeFactory = transactionScopeFactory;
            _mailService = mailService;
            _queryExecutor = queryExecutor;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _permissionValidationService = permissionValidationService;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _passwordCryptographyService = passwordCryptographyService;
            _passwordGenerationService = passwordGenerationService;
        }

        #endregion

        public async Task ExecuteAsync(ResetUserPasswordCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserAsync(command.UserId);
            ValidatePermissions(user, executionContext);
            ValidateUserArea(user.UserAreaCode);

            var temporaryPassword = _passwordGenerationService.Generate();

            var hashResult = _passwordCryptographyService.CreateHash(temporaryPassword);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
            user.RequirePasswordChange = true;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await SendNotificationAsync(user, temporaryPassword);

                await scope.CompleteAsync();
            }
        }

        private Task<User> GetUserAsync(int userId)
        {
            var user = _dbContext
                .Users
                .FilterById(userId)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();

            return user;
        }

        private void ValidateUserArea(string userAreaCode)
        {
            var userArea = _userAreaDefinitionRepository.GetByCode(userAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not allow password logins.");
            }

            if (!userArea.UseEmailAsUsername)
            {
                throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not require email addresses.");
            }
        }

        private async Task SendNotificationAsync(User user, string temporaryPassword)
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserAreaCode);
            
            var context = await CreateMailTemplateContextAsync(user, temporaryPassword);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordResetByAdminTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(user.Email, mailTemplate);
        }

        private async Task<PasswordResetByAdminTemplateBuilderContext> CreateMailTemplateContextAsync(
            User user,
            string temporaryPassword
            )
        {
            var query = new GetUserSummaryByIdQuery(user.UserId);
            var userSummary = await _queryExecutor.ExecuteAsync(query);
            EntityNotFoundException.ThrowIfNull(userSummary, user.UserId);

            var context = new PasswordResetByAdminTemplateBuilderContext()
            {
                User = userSummary,
                TemporaryPassword = new HtmlString(temporaryPassword)
            };

            return context;
        }

        #region Permission

        public void ValidatePermissions(User user, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetByCode(user.UserAreaCode);
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }
        }

        #endregion
    }
}
