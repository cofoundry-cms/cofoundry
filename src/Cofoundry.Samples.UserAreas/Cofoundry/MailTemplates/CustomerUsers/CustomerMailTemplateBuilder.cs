using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;
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
        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(INewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            // build the default template
            var template = await context.BuildDefaultTemplateAsync();

            // customize the subject, the {0} token is replaced with the application name
            template.SubjectFormat = "A new account has been created for you on {0}";

            return template;
        }

        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(IPasswordChangedTemplateBuilderContext context)
        {
            // build the default template
            var template = await context.BuildDefaultTemplateAsync();

            // customise the subject, you don't need to include the "{0}" token
            template.SubjectFormat = "Your password has been changed";

            return template;
        }

        public async Task<IMailTemplate> BuildPasswordResetTemplateAsync(IPasswordResetTemplateBuilderContext context)
        {
            // build the default template
            var template = await context.BuildDefaultTemplateAsync();

            // return unmodified
            return template;
        }

        public async Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(IAccountRecoveryTemplateBuilderContext context)
        {
            // build the default template
            var template = await context.BuildDefaultTemplateAsync();

            // customize the view file
            template.ViewFile = "~/Cofoundry/MailTemplates/CustomerUsers/Templates/AccountRecoveryMailTemplate";

            return template;
        }

        public async Task<IMailTemplate> BuildAccountVerificationTemplateAsync(IAccountVerificationTemplateBuilderContext context)
        {
            // build the default template
            var template = await context.BuildDefaultTemplateAsync();

            // customize the subject, the {0} token is replaced with the application name
            template.SubjectFormat = "Verify your account for {0}";

            // customize the view file
            template.ViewFile = "~/Cofoundry/MailTemplates/CustomerUsers/Templates/AccountVerificationMailTemplate";

            return template;
        }
    }
}