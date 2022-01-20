using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent to a new user when their account has
    /// been created with a temporary password. This version of the template 
    /// is used by default for the Cofoundry admin user area.
    /// </summary>
    public class AdminNewUserWithTemporaryPasswordMailTemplate : DefaultMailTemplateBase
    {
        public AdminNewUserWithTemporaryPasswordMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminNewUserWithTemporaryPasswordMailTemplate));
            SubjectFormat = "{0}: Your account has been created";
        }

        /// <summary>
        /// The username of the new user.
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
