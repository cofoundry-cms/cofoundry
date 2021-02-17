using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    public class DefaultMailTemplateBuilder<T> : IDefaultMailTemplateBuilder<T>
        where T : IUserAreaDefinition
    { 
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPasswordResetUrlHelper _passwordResetUrlHelper;
        private readonly IUserAreaDefinition _userAreaDefinition;

        public DefaultMailTemplateBuilder(
            IQueryExecutor queryExecutor,
            IPasswordResetUrlHelper passwordResetUrlHelper,
            T userAreaDefinition
            )
        {
            _queryExecutor = queryExecutor;
            _passwordResetUrlHelper = passwordResetUrlHelper;
            _userAreaDefinition = userAreaDefinition;
        }

        public virtual async Task<DefaultNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(
            NewUserWithTemporaryPasswordTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _userAreaDefinition.LoginPath;

            return new DefaultNewUserWithTemporaryPasswordMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = DefaultMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<DefaultPasswordResetByAdminMailTemplate> BuildPasswordResetByAdminTemplateAsync(
            PasswordResetByAdminTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _userAreaDefinition.LoginPath;

            return new DefaultPasswordResetByAdminMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = DefaultMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<DefaultPasswordResetRequestedByUserMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(
            PasswordResetRequestedByUserTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var resetUrl = _passwordResetUrlHelper.MakeUrl(context);

            return new DefaultPasswordResetRequestedByUserMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                ResetUrl = resetUrl,
                LayoutFile = DefaultMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<DefaultPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(
            PasswordChangedTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var loginPath = _userAreaDefinition.LoginPath;

            return new DefaultPasswordChangedMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginPath = loginPath,
                LayoutFile = DefaultMailTemplatePath.LayoutPath
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
