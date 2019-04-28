using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    /// <summary>
    /// A simple wrapper around IGenericMailTemplateBuilder for internal use
    /// which allows the IGenericMailTemplateBuilder to be easier to inject in
    /// custom implementation. It's a little verbose here but it makes overriding
    /// generic mail templates simpler.
    /// </summary>
    public class GenericMailTemplateBuilderWrapper<T> : IUserMailTemplateBuilder
        where T : IUserAreaDefinition
    { 
        private readonly IUserAreaDefinition _userAreaDefinition;
        private readonly IGenericMailTemplateBuilder<T> _genericMailTemplateBuilder;

        public GenericMailTemplateBuilderWrapper(
            IUserAreaDefinition userAreaDefinition,
            IGenericMailTemplateBuilder<T> genericMailTemplateBuilder
            )
        {
            _userAreaDefinition = userAreaDefinition;
            _genericMailTemplateBuilder = genericMailTemplateBuilder;
        }

        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            return await _genericMailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            return await _genericMailTemplateBuilder.BuildPasswordResetByAdminTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            return await _genericMailTemplateBuilder.BuildPasswordResetRequestedByUserTemplateAsync(context);
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            return await _genericMailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);
        }
    }
}
