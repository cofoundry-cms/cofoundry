using Cofoundry.Core.Web;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <inheritdoc/>
    public class CofoundryAdminMailTemplateBuilder : ICofoundryAdminMailTemplateBuilder
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly AdminSettings _adminSettings;
        private readonly ISiteUrlResolver _siteUrlResolver;

        public CofoundryAdminMailTemplateBuilder(
            IQueryExecutor queryExecutor,
            AdminSettings adminSettings,
            ISiteUrlResolver siteUrlResolver
            )
        {
            _queryExecutor = queryExecutor;
            _adminSettings = adminSettings;
            _siteUrlResolver = siteUrlResolver;
        }

        public virtual async Task<AdminNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginUrl = GetLoginUrl();

            return new AdminNewUserWithTemporaryPasswordMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginUrl = loginUrl,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = AdminMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<AdminPasswordResetMailTemplate> BuildPasswordResetTemplateAsync(PasswordResetTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginUrl = GetLoginUrl();

            return new AdminPasswordResetMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginUrl = loginUrl,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = AdminMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<AdminAccountRecoveryMailTemplate> BuildAccountRecoveryTemplateAsync(AccountRecoveryTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();

            if (context.RecoveryUrlPath == null || !Uri.IsWellFormedUriString(context.RecoveryUrlPath, UriKind.Relative))
            {
                throw new InvalidOperationException($"{nameof(AccountRecoveryTemplateBuilderContext)}.{nameof(context.RecoveryUrlPath)} should be a valid relative uri.");
            }

            return new AdminAccountRecoveryMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                RecoveryUrl = _siteUrlResolver.MakeAbsolute(context.RecoveryUrlPath),
                LayoutFile = AdminMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<AdminPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            var applicationName = await GetApplicationNameAsync();
            var loginUrl = GetLoginUrl();

            return new AdminPasswordChangedMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginUrl = loginUrl,
                LayoutFile = AdminMailTemplatePath.LayoutPath
            };
        }

        private string GetLoginUrl()
        {
            return _siteUrlResolver.MakeAbsolute("/" + _adminSettings.DirectoryName);
        }

        private async Task<string> GetApplicationNameAsync()
        {
            var query = new GetSettingsQuery<GeneralSiteSettings>();
            var result = await _queryExecutor.ExecuteAsync(query);

            return result?.ApplicationName;
        }
    }
}
