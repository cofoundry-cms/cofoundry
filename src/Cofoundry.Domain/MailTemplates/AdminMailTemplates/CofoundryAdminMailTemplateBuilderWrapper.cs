using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// A simple wrapper around ICofoundryAdminMailTemplateBuilder for internal use
    /// which allows the ICofoundryAdminMailTemplateBuilder to be easier to inject in
    /// custom implementation. It's a little verbose here but it makes overriding
    /// the mail templates simpler.
    /// </summary>
    public class CofoundryAdminMailTemplateBuilderWrapper : IUserMailTemplateBuilder
    { 
        private readonly ICofoundryAdminMailTemplateBuilder _cofoundryAdminMailTemplateBuilder;

        public CofoundryAdminMailTemplateBuilderWrapper(
            ICofoundryAdminMailTemplateBuilder cofoundryAdminMailTemplateBuilder
            )
        {
            _cofoundryAdminMailTemplateBuilder = cofoundryAdminMailTemplateBuilder;
        }

        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            return await _cofoundryAdminMailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            return await _cofoundryAdminMailTemplateBuilder.BuildPasswordResetByAdminTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            return await _cofoundryAdminMailTemplateBuilder.BuildPasswordResetRequestedByUserTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            return await _cofoundryAdminMailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);
        }
    }
}
