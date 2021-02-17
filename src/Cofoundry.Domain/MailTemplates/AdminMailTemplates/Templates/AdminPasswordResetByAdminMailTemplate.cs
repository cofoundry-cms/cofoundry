using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent when an administrator resets a users 
    /// password either programatically or via the admin panel. This
    /// version of the template is used by default for the Cofoundry
    /// admin user area.
    /// </summary>
    public class AdminPasswordResetByAdminMailTemplate : DefaultMailTemplateBase
    {
        public AdminPasswordResetByAdminMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminPasswordResetByAdminMailTemplate));
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
        /// The path that the user can use to log in.
        /// </summary>
        public string LoginPath { get; set; }
    }
}
