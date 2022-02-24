namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent to a user to notify them that their
    /// password has been changed. This version of the template is used 
    /// by default for the Cofoundry admin user area.
    /// </summary>
    public class AdminPasswordChangedMailTemplate : DefaultMailTemplateBase
    {
        public AdminPasswordChangedMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminPasswordChangedMailTemplate));
            SubjectFormat = "{0}: Password changed";
        }

        /// <summary>
        /// The user who has had their password changed.
        /// </summary>
        public UserSummary User { get; set; }

        /// <summary>
        /// The absolute sign in page url e.g. "https://www.example.com/admin/".
        /// </summary>
        public string SignInUrl { get; set; }
    }
}