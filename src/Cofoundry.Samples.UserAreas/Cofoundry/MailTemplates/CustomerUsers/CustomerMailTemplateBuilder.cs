using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;
using Cofoundry.Domain.MailTemplates.GenericMailTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    /// <summary>
    /// An example of creating a custom IUserMailTemplateBuilder for the customer user 
    /// area and making use of IGenericMailTemplateBuilder to make minor modifications
    /// to the built-in generic templates instead of starting from scratch.
    /// </summary>
    public class CustomerMailTemplateBuilder : IUserMailTemplateBuilder<CustomerUserAreaDefinition>
    {
        private readonly IGenericMailTemplateBuilder<CustomerUserAreaDefinition> _genericMailTemplateBuilder;

        public CustomerMailTemplateBuilder(
            IGenericMailTemplateBuilder<CustomerUserAreaDefinition> genericMailTemplateBuilder
            ) 
        {
            _genericMailTemplateBuilder = genericMailTemplateBuilder;
        }

        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            // build the generic template
            var template = await _genericMailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);

            // customise the subject, the {0} token is replaced with the application name
            template.SubjectFormat = "A new account has been created for you on {0}";

            return template;
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context)
        {
            // build the generic template
            var template = await _genericMailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);

            // customise the subject, you don't need to include the "{0}" token
            template.SubjectFormat = "Your password has been changed";

            return template;
        }

        public async Task<IMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context)
        {
            // build the generic template
            var template = await _genericMailTemplateBuilder.BuildPasswordResetByAdminTemplateAsync(context);

            // return unmodified
            return template;
        }

        public async Task<IMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            // build the generic template
            var template = await _genericMailTemplateBuilder.BuildPasswordResetRequestedByUserTemplateAsync(context);

            // customize the view file
            template.ViewFile = "~/Cofoundry/MailTemplates/CustomerUsers/Templates/PasswordResetRequestedByUserMailTemplate";

            return template;
        }
    }
}
