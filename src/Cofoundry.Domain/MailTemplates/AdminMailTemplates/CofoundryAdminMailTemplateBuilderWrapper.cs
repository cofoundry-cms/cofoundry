using Cofoundry.Core.Mail;
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

        public async Task<IMailTemplate> BuildPasswordResetTemplateAsync(PasswordResetTemplateBuilderContext context)
        {
            return await _cofoundryAdminMailTemplateBuilder.BuildPasswordResetTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(AccountRecoveryTemplateBuilderContext context)
        {
            return await _cofoundryAdminMailTemplateBuilder.BuildAccountRecoveryTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            return await _cofoundryAdminMailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);
        }
    }
}
