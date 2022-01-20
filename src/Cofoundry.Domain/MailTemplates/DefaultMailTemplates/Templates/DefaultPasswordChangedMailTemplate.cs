namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Template for the email sent to a user to notify them that their
    /// password has been changed. This default version of the template 
    /// is used for all user areas except the Cofoundry admin user area.
    /// </summary>
    public class DefaultPasswordChangedMailTemplate : DefaultMailTemplateBase
    {
        public DefaultPasswordChangedMailTemplate()
        {
            SubjectFormat = "{0}: Password changed";
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultPasswordChangedMailTemplate));
        }

        /// <summary>
        /// The username of the user who has had their password changed.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The absolute login page url e.g. "https://www.example.com/members/login".
        /// </summary>
        public string LoginUrl { get; set; }
    }
}