using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly IUserAreaDefinition _userAreaDefinition;
        private readonly IDefaultMailTemplateBuilder<T> _defaultMailTemplateBuilder;

        public DefaultMailTemplateBuilderWrapper(
            IUserAreaDefinition userAreaDefinition,
            IDefaultMailTemplateBuilder<T> defaultMailTemplateBuilder
            )
        {
            _userAreaDefinition = userAreaDefinition;
            _defaultMailTemplateBuilder = defaultMailTemplateBuilder;
        }

        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildPasswordResetByAdminTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildPasswordResetRequestedByUserTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            return await _defaultMailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);
        }
    }
}
