namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests to resets their 
    /// own password which is usually via a 'forgot password' mechanism on
    /// a sign in page. This default version of the template is used for all 
    /// user areas except the Cofoundry admin user area.
    /// </summary>
    public class AccountRecoveryMailTemplate : UserMailTemplateBase
    {
        public AccountRecoveryMailTemplate()
        {
            LayoutFile = DefaultMailTemplatePath.LayoutPath;
            SubjectFormat = "{0}: Account recovery request";
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(AccountRecoveryMailTemplate));
        }

        /// <summary>
        /// The user requesting to recover their account.
        /// </summary>
        public override UserSummary User { get; set; }

        /// <summary>
        /// The absolute url for the account recovery completion form e.g.
        /// "https://example.com/auth/acount-recovery?t={token}".
        /// </summary>
        public string RecoveryUrl { get; set; }
    }
}