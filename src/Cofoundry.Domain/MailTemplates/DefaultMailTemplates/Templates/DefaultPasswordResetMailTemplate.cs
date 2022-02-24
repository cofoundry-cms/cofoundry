using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Template for the email sent when an administrator resets a users 
    /// password either programatically or via the admin panel. This default
    /// version of the template is used for all user areas except the Cofoundry 
    /// admin user area.
    /// </summary>
    public class DefaultPasswordResetMailTemplate : DefaultMailTemplateBase
    {
        public DefaultPasswordResetMailTemplate()
        {
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultPasswordResetMailTemplate));
            SubjectFormat = "{0}: Your password has been reset";
        }

        /// <summary>
        /// The user who has had their password reset.
        /// </summary>
        public UserSummary User { get; set; }

        /// <summary>
        /// The temporary password that the user can use to log in to 
        /// the site.
        /// </summary>
        public IHtmlContent TemporaryPassword { get; set; }

        /// <summary>
        /// The absolute sign in page url e.g. "https://www.example.com/members/sign-in".
        /// </summary>
        public string SignInUrl { get; set; }
    }
}
