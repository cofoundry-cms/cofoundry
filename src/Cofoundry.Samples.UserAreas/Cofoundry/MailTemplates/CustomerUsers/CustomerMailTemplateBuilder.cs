using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;
using Cofoundry.Domain.MailTemplates.DefaultMailTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    /// <summary>
    /// An example of creating a custom IUserMailTemplateBuilder for the customer user 
    /// area and making use of IDefaultMailTemplateBuilder to make minor modifications
    /// to the built-in default templates instead of starting from scratch.
    /// </summary>
    public class CustomerMailTemplateBuilder : IUserMailTemplateBuilder<CustomerUserArea>
    {
        private readonly IDefaultMailTemplateBuilder<CustomerUserArea> _defaultMailTemplateBuilder;

        public CustomerMailTemplateBuilder(
            IDefaultMailTemplateBuilder<CustomerUserArea> defaultMailTemplateBuilder
            ) 
        {
            _defaultMailTemplateBuilder = defaultMailTemplateBuilder;
        }

        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            // build the default template
            var template = await _defaultMailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);

            // customize the subject, the {0} token is replaced with the application name
            template.SubjectFormat = "A new account has been created for you on {0}";

            return template;
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            // build the default template
            var template = await _defaultMailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);

            // customise the subject, you don't need to include the "{0}" token
            template.SubjectFormat = "Your password has been changed";

            return template;
        }

        public async Task<IMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            // build the default template
            var template = await _defaultMailTemplateBuilder.BuildPasswordResetByAdminTemplateAsync(context);

            // return unmodified
            return template;
        }

        public async Task<IMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            // build the default template
            var template = await _defaultMailTemplateBuilder.BuildPasswordResetRequestedByUserTemplateAsync(context);

            // customize the view file
            template.ViewFile = "~/Cofoundry/MailTemplates/CustomerUsers/Templates/PasswordResetRequestedByUserMailTemplate";

            return template;
        }
    }
}
