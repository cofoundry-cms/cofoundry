using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    public class GenericMailTemplateBuilder : IUserMailTemplateBuilder
    { 
        private readonly IUserAreaDefinition _userAreaDefinition;
        private readonly IQueryExecutor _queryExecutor;

        public GenericMailTemplateBuilder(
            IUserAreaDefinition userAreaDefinition,
            IQueryExecutor queryExecutor
            )
        {
            _userAreaDefinition = userAreaDefinition;
            _queryExecutor = queryExecutor;
        }

        public virtual async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _userAreaDefinition.LoginPath;

            return new GenericNewUserWithTemporaryPasswordMailTemplate()
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
            var loginPath = _userAreaDefinition.LoginPath;

            return new GenericPasswordResetByAdminMailTemplate()
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
            var resetUrl = GetPasswordResetUrl(_userAreaDefinition, context);

            return new GenericPasswordResetRequestedByUserMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                ResetUrl = resetUrl
            };
        }

        public virtual async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _userAreaDefinition.LoginPath;

            return new GenericPasswordChangedMailTemplate()
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

        protected virtual string GetPasswordResetUrl(IUserAreaDefinition userAreaDefinition, PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            var baseUrl = RelativePathHelper.Combine(userAreaDefinition.LoginPath, "reset-password");

            return string.Format("{0}?i={1}&t={2}",
                baseUrl,
                context.UserPasswordResetRequestId.ToString("N"),
                Uri.EscapeDataString(context.Token));
        }
    }
}
