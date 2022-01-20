using Cofoundry.Core.Web;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <inheritdoc/>
    public class DefaultMailTemplateBuilder<T> : IDefaultMailTemplateBuilder<T>
        where T : IUserAreaDefinition
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserAreaDefinition _userAreaDefinition;
        private readonly ISiteUrlResolver _siteUrlResolver;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public DefaultMailTemplateBuilder(
            IQueryExecutor queryExecutor,
            T userAreaDefinition,
            ISiteUrlResolver siteUrlResolver,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _queryExecutor = queryExecutor;
            _userAreaDefinition = userAreaDefinition;
            _siteUrlResolver = siteUrlResolver;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public virtual async Task<DefaultNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(
            NewUserWithTemporaryPasswordTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var loginUrl = GetLoginUrl();

            return new DefaultNewUserWithTemporaryPasswordMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginUrl = loginUrl,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = DefaultMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<DefaultPasswordResetMailTemplate> BuildPasswordResetTemplateAsync(
            PasswordResetTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var loginUrl = GetLoginUrl();

            return new DefaultPasswordResetMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginUrl = loginUrl,
                TemporaryPassword = context.TemporaryPassword,
                LayoutFile = DefaultMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<DefaultAccountRecoveryMailTemplate> BuildAccountRecoveryTemplateAsync(
            AccountRecoveryTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();

            if (context.RecoveryUrlPath == null || !Uri.IsWellFormedUriString(context.RecoveryUrlPath, UriKind.Relative))
            {
                // The RecoveryUrlPath setting isn't required in config because the feature may not be used, or may 
                // be manually constructed in a custom IDefaultMailTemplateBuilder implementation. However it should be 
                // supplied at this point for our default builder.
                var options = _userAreaDefinitionRepository.GetOptionsByCode(_userAreaDefinition.UserAreaCode).AccountRecovery;
                if (string.IsNullOrEmpty(options.RecoveryUrlBase))
                {
                    throw new InvalidOperationException($"To use the account recovery feature you must configure the {nameof(AccountRecoveryOptions.RecoveryUrlBase)} setting by implementing {nameof(IUserAreaDefinition)}.{nameof(IUserAreaDefinition.ConfigureOptions)}.");
                }

                throw new InvalidOperationException($"{nameof(AccountRecoveryTemplateBuilderContext)}.{nameof(context.RecoveryUrlPath)} should be a valid relative uri.");
            }

            return new DefaultAccountRecoveryMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                RecoveryUrl = _siteUrlResolver.MakeAbsolute(context.RecoveryUrlPath),
                LayoutFile = DefaultMailTemplatePath.LayoutPath
            };
        }

        public virtual async Task<DefaultPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(
            PasswordChangedTemplateBuilderContext context
            )
        {
            var applicationName = await GetApplicationNameAsync();
            var loginUrl = GetLoginUrl();

            return new DefaultPasswordChangedMailTemplate()
            {
                Username = context.User.Username,
                ApplicationName = applicationName,
                LoginUrl = loginUrl,
                LayoutFile = DefaultMailTemplatePath.LayoutPath
            };
        }

        private string GetLoginUrl()
        {
            return _siteUrlResolver.MakeAbsolute(_userAreaDefinition.LoginPath);
        }

        private async Task<string> GetApplicationNameAsync()
        {
            var query = new GetSettingsQuery<GeneralSiteSettings>();
            var result = await _queryExecutor.ExecuteAsync(query);

            return result?.ApplicationName;
        }
    }
}
