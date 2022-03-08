using Cofoundry.Domain.MailTemplates.Internal;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Template for the email sent to a user to notify them that their
    /// password has been changed. This default version of the template 
    /// is used for all user areas except the Cofoundry admin user area.
    /// </summary>
    public class PasswordChangedMailTemplate : UserMailTemplateBase, IMailTemplateWithSignInUrl
    {
        public PasswordChangedMailTemplate()
        {
            LayoutFile = DefaultMailTemplatePath.LayoutPath;
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(PasswordChangedMailTemplate));
            SubjectFormat = "{0}: Password changed";
        }

        /// <summary>
        /// The user that has had their password changed.
        /// </summary>
        public override UserSummary User { get; set; }

        /// <summary>
        /// The absolute sign in page url e.g. "https://www.example.com/members/sign-in".
        /// </summary>
        public string SignInUrl { get; set; }
    }
}