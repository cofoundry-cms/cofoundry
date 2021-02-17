using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    public class CofoundryAdminMailTemplateBuilder : ICofoundryAdminMailTemplateBuilder
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly AdminMailTemplateUrlLibrary _adminMailTemplateUrlLibrary;
        private readonly IPasswordResetUrlHelper _passwordResetUrlHelper;

        public CofoundryAdminMailTemplateBuilder(
            IQueryExecutor queryExecutor,
            AdminMailTemplateUrlLibrary adminMailTemplateUrlLibrary,
            IPasswordResetUrlHelper passwordResetUrlHelper
            )
        {
            _queryExecutor = queryExecutor;
            _adminMailTemplateUrlLibrary = adminMailTemplateUrlLibrary;
            _passwordResetUrlHelper = passwordResetUrlHelper;
        }

        public virtual async Task<AdminNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _adminMailTemplateUrlLibrary.Login();

            return new AdminNewUserWithTemporaryPasswordMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = AdminMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<AdminPasswordResetByAdminMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _adminMailTemplateUrlLibrary.Login();

            return new AdminPasswordResetByAdminMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = AdminMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<AdminPasswordResetRequestedByUserMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var resetUrl = _passwordResetUrlHelper.MakeUrl(context);

            return new AdminPasswordResetRequestedByUserMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                ResetUrl = resetUrl,
                LayoutFile = AdminMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<AdminPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _adminMailTemplateUrlLibrary.Login();

            return new AdminPasswordChangedMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                LayoutFile = AdminMailTemplatePath.LayoutPath
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
