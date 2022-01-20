using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent when an administrator resets a users 
    /// password either programatically or via the admin panel. This
    /// version of the template is used by default for the Cofoundry
    /// admin user area.
    /// </summary>
    public class AdminPasswordResetMailTemplate : DefaultMailTemplateBase
    {
        public AdminPasswordResetMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminPasswordResetMailTemplate));
            SubjectFormat = "{0}: Your password has been reset";
        }

        /// <summary>
        /// The username of the user who has had their password reset.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The temporary password that the user can use to log in to 
        /// the site.
        /// </summary>
        public IHtmlContent TemporaryPassword { get; set; }

        /// <summary>
        /// The absolute login page url e.g. "https://www.example.com/admin/".
        /// </summary>
        public string LoginUrl { get; set; }
    }
}
