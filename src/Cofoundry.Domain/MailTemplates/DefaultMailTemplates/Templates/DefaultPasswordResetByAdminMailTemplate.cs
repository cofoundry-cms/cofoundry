using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Template for the email sent when an administrator resets a users 
    /// password either programatically or via the admin panel. This default
    /// version of the template is used for all user areas except the Cofoundry 
    /// admin user area.
    /// </summary>
    public class DefaultPasswordResetByAdminMailTemplate : DefaultMailTemplateBase
    {
        public DefaultPasswordResetByAdminMailTemplate()
        {
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultPasswordResetByAdminMailTemplate));
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
