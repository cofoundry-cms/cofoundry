namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user is required to verify their 
    /// account which typically is done during a custom user sign-up process. 
    /// This default version of the template is used for all user areas except 
    /// the Cofoundry admin user area.
    /// </summary>
    public class DefaultAccountVerificationMailTemplate : DefaultMailTemplateBase
    {
        public DefaultAccountVerificationMailTemplate()
        {
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultAccountVerificationMailTemplate));
            SubjectFormat = "{0}: Please verify your account";
        }

        /// <summary>
        /// The username of the user verifying their account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The absolute url for the account verification page e.g.
        /// "https://example.com/auth/verify-acount?t={token}".
        /// </summary>
        public string VerificationUrl { get; set; }
    }
}
