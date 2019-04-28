using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    public class GenericMailTemplateBuilder<T> : IGenericMailTemplateBuilder<T>
        where T : IUserAreaDefinition
    { 
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPasswordResetUrlHelper _passwordResetUrlHelper;
        private readonly IUserAreaDefinition _userAreaDefinition;

        public GenericMailTemplateBuilder(
            IQueryExecutor queryExecutor,
            IPasswordResetUrlHelper passwordResetUrlHelper,
            T userAreaDefinition
            )
        {
            _queryExecutor = queryExecutor;
            _passwordResetUrlHelper = passwordResetUrlHelper;
            _userAreaDefinition = userAreaDefinition;
        }

        public virtual async Task<GenericNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(
            NewUserWithTemporaryPasswordTemplateBuilderContext context
            )
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

        public async Task<GenericPasswordResetByAdminMailTemplate> BuildPasswordResetByAdminTemplateAsync(
            PasswordResetByAdminTemplateBuilderContext context
            )
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

        public virtual async Task<GenericPasswordResetRequestedByUserMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(
            PasswordResetRequestedByUserTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var resetUrl = _passwordResetUrlHelper.MakeUrl(context);

            return new GenericPasswordResetRequestedByUserMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                ResetUrl = resetUrl
            };
        }

        public virtual async Task<GenericPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(
            PasswordChangedTemplateBuilderContext context
            )
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
    }
}
