namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests to resets their 
    /// own password which is usually via a 'forgot password' mechanism on
    /// a sign in page. This default version of the template is used for all 
    /// user areas except the Cofoundry admin user area.
    /// </summary>
    public class DefaultAccountRecoveryMailTemplate : DefaultMailTemplateBase
    {
        public DefaultAccountRecoveryMailTemplate()
        {
            SubjectFormat = "{0}: Account recovery request";
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultAccountRecoveryMailTemplate));
        }

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