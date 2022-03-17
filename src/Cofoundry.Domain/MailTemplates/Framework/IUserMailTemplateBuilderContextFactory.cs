namespace Cofoundry.Domain.MailTemplates.Internal;

/// <summary>
/// Factory that abstracts away some of the complexities of building the
/// context object for each of the built-in mail templates.
/// </summary>
public interface IUserMailTemplateBuilderContextFactory
{
    IAccountRecoveryTemplateBuilderContext CreateAccountRecoveryContext(UserSummary user, string token);

    IAccountVerificationTemplateBuilderContext CreateAccountVerificationContext(UserSummary user, string token);

    INewUserWithTemporaryPasswordTemplateBuilderContext CreateNewUserWithTemporaryPasswordContext(UserSummary user, string temporaryPassword);

    IPasswordChangedTemplateBuilderContext CreatePasswordChangedContext(UserSummary user);

    IPasswordResetTemplateBuilderContext CreatePasswordResetContext(UserSummary user, string temporaryPassword);
}
