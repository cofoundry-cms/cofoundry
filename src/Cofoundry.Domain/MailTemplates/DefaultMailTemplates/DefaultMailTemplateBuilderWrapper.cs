using Cofoundry.Core.Mail;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// A simple wrapper around IDefaultMailTemplateBuilder for internal use
    /// which allows the IDefaultMailTemplateBuilder to be easier to inject in
    /// custom implementation. It's a little verbose here but it makes overriding
    /// default mail templates simpler.
    /// </summary>
    public class DefaultMailTemplateBuilderWrapper<T> : IUserMailTemplateBuilder
        where T : IUserAreaDefinition
    {
        private readonly IDefaultMailTemplateBuilder<T> _defaultMailTemplateBuilder;

        public DefaultMailTemplateBuilderWrapper(
            IDefaultMailTemplateBuilder<T> defaultMailTemplateBuilder
            )
        {
            _defaultMailTemplateBuilder = defaultMailTemplateBuilder;
        }

        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordResetTemplateAsync(PasswordResetTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildPasswordResetTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(AccountRecoveryTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildAccountRecoveryTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildAccountVerificationTemplateAsync(AccountVerificationTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildAccountVerificationTemplateAsync(context);
        }
    }
}