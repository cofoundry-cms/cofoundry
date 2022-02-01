using Cofoundry.Core.Mail;
using Cofoundry.Core.Web;
using Cofoundry.Domain.MailTemplates;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    /// <summary>
    /// An example of how to completely customize mail templates generated
    /// by Cofoundry for a user area. 
    /// 
    /// Each method represents a specific email sent out by the system and 
    /// should return an IMailTemplate instance which will be rendered by 
    /// Cofoundry. The methods are async so you have the freedom to fetch 
    /// and mix in any additional data you need.
    /// </summary>
    public class PartnerMailTemplateBuilder : IUserMailTemplateBuilder<PartnerUserArea>
    {
        private readonly ISiteUrlResolver _siteUrlResolver;

        public PartnerMailTemplateBuilder(
            ISiteUrlResolver siteUrlResolver
            )
        {
            _siteUrlResolver = siteUrlResolver;
        }

        public Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            var template = new NewUserWithTemporaryPasswordMailTemplate()
            {
                Username = context.User.Username,
                LoginUrl = GetLoginUrl(),
                TemporaryPassword = context.TemporaryPassword
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        public Task<IMailTemplate> BuildPasswordResetTemplateAsync(PasswordResetTemplateBuilderContext context)
        {
            var template = new PasswordResetMailTemplate()
            {
                Username = context.User.Username,
                LoginUrl = GetLoginUrl(),
                TemporaryPassword = context.TemporaryPassword
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        public Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(AccountRecoveryTemplateBuilderContext context)
        {
            var template = new AccountRecoveryMailTemplate()
            {
                Username = context.User.Username,
                RecoveryUrl = _siteUrlResolver.MakeAbsolute(context.RecoveryUrlPath)
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        public Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            var template = new PasswordChangedMailTemplate()
            {
                Username = context.User.Username,
                LoginUrl = GetLoginUrl()
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        public Task<IMailTemplate> BuildAccountVerificationTemplateAsync(AccountVerificationTemplateBuilderContext context)
        {
            // Partner accounts don't require verification
            throw new NotSupportedException();
        }

        private string GetLoginUrl()
        {
            return _siteUrlResolver.MakeAbsolute(UrlLibrary.PartnerLogin());
        }
    }
}
