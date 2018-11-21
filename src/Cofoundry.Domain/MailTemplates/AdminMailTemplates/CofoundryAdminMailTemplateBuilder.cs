using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    public class CofoundryAdminMailTemplateBuilder : IUserMailTemplateBuilder
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly AdminMailTemplateUrlLibrary _adminMailTemplateUrlLibrary;

        public CofoundryAdminMailTemplateBuilder(
            IQueryExecutor queryExecutor,
            AdminMailTemplateUrlLibrary adminMailTemplateUrlLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _adminMailTemplateUrlLibrary = adminMailTemplateUrlLibrary;
        }

        public virtual async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _adminMailTemplateUrlLibrary.Login();

            return new AdminNewUserWithTemporaryPasswordMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                TemporaryPassword = context.TemporaryPassword
            };
        }

        public async Task<IMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _adminMailTemplateUrlLibrary.Login();

            return new AdminPasswordResetByAdminMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                TemporaryPassword = context.TemporaryPassword
            };
        }

        public virtual async Task<IMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var url = _adminMailTemplateUrlLibrary.PasswordReset(context);

            return new AdminPasswordResetRequestedByUserMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                ResetUrl = url
            };
        }

        public virtual async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _adminMailTemplateUrlLibrary.Login();

            return new AdminPasswordChangedMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath
            };
        }

        private async Task<string> GetApplicationNameAsync()
        {
            var query = new GetSettingsQuery<GeneralSiteSettings>();
            var result = await _queryExecutor.ExecuteAsync(query);

            return result?.ApplicationName;
        }
    }
}
