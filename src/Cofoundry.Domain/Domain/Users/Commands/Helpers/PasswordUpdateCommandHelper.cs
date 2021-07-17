using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper used by in password update commands for shared functionality.
    /// </summary>
    public class PasswordUpdateCommandHelper : IPasswordUpdateCommandHelper
    {
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IMailService _mailService;

        public PasswordUpdateCommandHelper(
            IPermissionValidationService permissionValidationService,
            IPasswordCryptographyService passwordCryptographyService,
            IMailService mailService,
            IQueryExecutor queryExecutor,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory
            )
        {
            _passwordCryptographyService = passwordCryptographyService;
            _permissionValidationService = permissionValidationService;
            _mailService = mailService;
            _queryExecutor = queryExecutor;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
        }

        public void ValidateUserArea(IUserAreaDefinition userArea)
        {
            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException("Cannot update the password to account in a user area that does not allow password logins.");
            }
        }

        public void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }
        }

        public void UpdatePassword(string newPassword, User user, IExecutionContext executionContext)
        {
            user.RequirePasswordChange = false;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;

            UpdatePasswordHash(newPassword, user);
        }

        public void UpdatePasswordHash(string newPassword, User user)
        {
            var hashResult = _passwordCryptographyService.CreateHash(newPassword);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
        }

        /// <summary>
        /// Send a notification to the user to let them know their 
        /// password has been changed. The template is built using the
        /// registered UserMailTemplateBuilderFactory for the users
        /// user area.
        /// </summary>
        /// <param name="user">The user to send the notification to.</param>
        public async Task SendPasswordChangedNotification(User user)
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(user.UserId);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;
             
            await _mailService.SendAsync(user.Email, mailTemplate);
        }

        private async Task<PasswordChangedTemplateBuilderContext> CreateMailTemplateContextAsync(int userId)
        {
            var query = new GetUserSummaryByIdQuery(userId);
            var user = await _queryExecutor.ExecuteAsync(query);
            EntityNotFoundException.ThrowIfNull(user, userId);

            var context = new PasswordChangedTemplateBuilderContext()
            {
                User = user
            };

            return context;
        }
    }
}
