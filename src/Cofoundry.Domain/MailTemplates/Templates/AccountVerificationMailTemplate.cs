namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Template for the email sent when a user is required to verify their 
    /// account which typically is done during a custom user sign-up process. 
    /// This default version of the template is used for all user areas except 
    /// the Cofoundry admin user area.
    /// </summary>
    public class AccountVerificationMailTemplate : UserMailTemplateBase
    {
        public AccountVerificationMailTemplate()
        {
            LayoutFile = DefaultMailTemplatePath.LayoutPath;
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(AccountVerificationMailTemplate));
            SubjectFormat = "{0}: Please verify your account";
        }

        /// <summary>
        /// The user verifying their account.
        /// </summary>
        public override UserSummary User { get; set; }

        /// <summary>
        /// The absolute url for the account verification page e.g.
        /// "https://example.com/auth/verify-acount?t={token}".
        /// </summary>
        public string VerificationUrl { get; set; }
    }
}
