using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    public class PartnerMailTemplateBuilder : IUserMailTemplateBuilder<PartnerUserAreaDefinition>
    { 
        public Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            var template = new NewUserWithTemporaryPasswordMailTemplate()
            {
                Username = context.User.Username,
                LoginPath = UrlLibrary.PartnerLogin(),
                TemporaryPassword = context.TemporaryPassword
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        public Task<IMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            var template = new PasswordResetByAdminMailTemplate()
            {
                Username = context.User.Username,
                LoginPath = UrlLibrary.PartnerLogin(),
                TemporaryPassword = context.TemporaryPassword
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        public Task<IMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            var resetUrl = "";// todo GetPasswordResetUrl(_userAreaDefinition, context);

            var template = new PasswordResetRequestedByUserMailTemplate()
            {
                Username = context.User.Username,
                ResetUrl = resetUrl
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        public Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            var template = new PasswordChangedMailTemplate()
            {
                Username = context.User.Username,
                LoginPath = UrlLibrary.PartnerLogin()
            };

            return Task.FromResult<IMailTemplate>(template);
        }

        //protected string GetPasswordResetUrl(IUserAreaDefinition userAreaDefinition, PasswordResetRequestedByUserTemplateBuilderContext context)
        //{
        //    var baseUrl = RelativePathHelper.Combine(userAreaDefinition.LoginPath, "reset-password");

        //    return string.Format("{0}?i={1}&t={2}",
        //        baseUrl,
        //        context.UserPasswordResetRequestId.ToString("N"),
        //        Uri.EscapeDataString(context.Token));
        //}
    }
}
