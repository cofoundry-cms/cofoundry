using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests to resets their 
    /// own password via the 'forgot password' mechanism on login form. 
    /// This version of the template is used by default for the Cofoundry
    /// admin user area.
    /// </summary>
    public class AdminPasswordResetRequestedByUserMailTemplate : DefaultMailTemplateBase
    {
        public AdminPasswordResetRequestedByUserMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminPasswordResetRequestedByUserMailTemplate));
            SubjectFormat = "{0}: Password reset request";
        }

        /// <summary>
        /// The username of the user who is requesting to have their 
        /// password reset.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The url that the user should follow to complete the password reset.
        /// </summary>
        public string ResetUrl { get; set; }
    }
}