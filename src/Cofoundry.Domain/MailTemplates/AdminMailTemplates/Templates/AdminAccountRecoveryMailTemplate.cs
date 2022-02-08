namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests an account recovery 
    /// via the 'forgot password' mechanism on a sign in form. This version of the template
    /// is used by default for the Cofoundry admin user area.
    /// </summary>
    public class AdminAccountRecoveryMailTemplate : DefaultMailTemplateBase
    {
        public AdminAccountRecoveryMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminAccountRecoveryMailTemplate));
            SubjectFormat = "{0}: Account recovery request";
        }

        /// <summary>
        /// The username of the user requesting to recover their account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The absolute url for the account recovery completion form e.g.
        /// "https://example.com/admin/account-recovery?t={token}".
        /// </summary>
        public string RecoveryUrl { get; set; }
    }
}