using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;

namespace AuthenticationSample;

/// <summary>
/// A custom IUserMailTemplateBuilder implementation allows us to override the
/// default mail templates sent by Cofoundry. Because we only need to return an
/// IMailTemplate, we can opt-in to change as much as we need to. Here we rely
/// on the default builder to generate the template models, which we then modify to
/// use custom layouts, view files and subjects.
/// </summary>
public class MemberMailTemplateBuilder : IUserMailTemplateBuilder<MemberUserArea>
{
    const string LAYOUT_FILE = "~/Cofoundry/MailTemplates/Layouts/_Layout";

    public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(INewUserWithTemporaryPasswordTemplateBuilderContext context)
    {
        // build the default template
        var template = await context.BuildDefaultTemplateAsync();

        // customize the subject, the optional {0} token is replaced with the application name
        template.SubjectFormat = "Your new member account on {0}";

        // customize the view file base
        template.ViewFile = "~/Cofoundry/MailTemplates/Templates/NewUserWithTemporaryPassword";

        // customize the layout file base
        template.LayoutFile = LAYOUT_FILE;

        return template;
    }

    public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(IPasswordChangedTemplateBuilderContext context)
    {
        var template = await context.BuildDefaultTemplateAsync();
        template.SubjectFormat = "Your password has been changed";
        template.ViewFile = "~/Cofoundry/MailTemplates/Templates/PasswordChanged";
        template.LayoutFile = LAYOUT_FILE;

        return template;
    }

    public async Task<IMailTemplate> BuildPasswordResetTemplateAsync(IPasswordResetTemplateBuilderContext context)
    {
        var template = await context.BuildDefaultTemplateAsync();
        template.ViewFile = "~/Cofoundry/MailTemplates/Templates/PasswordReset";
        template.LayoutFile = LAYOUT_FILE;

        return template;
    }

    public async Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(IAccountRecoveryTemplateBuilderContext context)
    {
        var template = await context.BuildDefaultTemplateAsync();
        template.ViewFile = "~/Cofoundry/MailTemplates/Templates/AccountRecovery";
        template.LayoutFile = LAYOUT_FILE;

        return template;
    }

    public Task<IMailTemplate> BuildAccountVerificationTemplateAsync(IAccountVerificationTemplateBuilderContext context)
    {
        // This sample does not use account verification, so we can safely ignore 
        // or throw an exception
        throw new NotImplementedException();
    }
}