using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;

namespace RegistrationAndVerificationSample;

/// <summary>
/// A custom IUserMailTemplateBuilder implementation allows us to override the
/// default mail templates sent by Cofoundry. Here we're just overriding the 
/// account verification templte. The AuthenticationSample contains examples
/// on overriding the other templates.
/// </summary>
public class MemberMailTemplateBuilder : IUserMailTemplateBuilder<MemberUserArea>
{
    public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(INewUserWithTemporaryPasswordTemplateBuilderContext context)
    {
        // Use the default template. See AuthenticationSample for an example of customizing this
        return await context.BuildDefaultTemplateAsync();
    }

    public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(IPasswordChangedTemplateBuilderContext context)
    {
        // Use the default template. See AuthenticationSample for an example of customizing this
        return await context.BuildDefaultTemplateAsync();
    }

    public async Task<IMailTemplate> BuildPasswordResetTemplateAsync(IPasswordResetTemplateBuilderContext context)
    {
        // Use the default template. See AuthenticationSample for an example of customizing this
        return await context.BuildDefaultTemplateAsync();
    }

    public async Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(IAccountRecoveryTemplateBuilderContext context)
    {
        // Use the default template. See AuthenticationSample for an example of customizing this
        return await context.BuildDefaultTemplateAsync();
    }

    public async Task<IMailTemplate> BuildAccountVerificationTemplateAsync(IAccountVerificationTemplateBuilderContext context)
    {
        // build the default template
        var template = await context.BuildDefaultTemplateAsync();

        // customize the subject, the {0} token is replaced with the application name
        template.SubjectFormat = "Verify your account for {0}";

        // customize the view file
        template.ViewFile = "~/Cofoundry/MailTemplates/Templates/AccountVerification";

        return template;
    }
}