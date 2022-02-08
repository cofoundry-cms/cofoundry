using Cofoundry.Core.Mail;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests to recover their
    /// account via the 'forgot password' mechanism on partner users
    /// sign in form.
    /// </summary>
    public class AccountRecoveryMailTemplate : IMailTemplate
    {
        /// <summary>
        /// Full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added. E.g. "~/Cofoundry/MailTemplates/MyMailTemplate".
        /// </summary>
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(AccountRecoveryMailTemplate));

        /// <summary>
        /// String to use as the subject to the email.
        /// </summary>
        public string Subject { get; } = "Account recovery request";

        /// <summary>
        /// The username of the user requesting to recover their account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The absolute url for the account recovery completion form e.g.
        /// "https://example.com/auth/acount-recovery?t={token}".
        /// </summary>
        public string RecoveryUrl { get; set; }
    }
}