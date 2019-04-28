using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    /// <summary>
    /// A custom template for the email sent when an administrator resets a 
    /// users password via the admin panel.
    /// </summary>
    public class PasswordResetByAdminMailTemplate : IMailTemplate
    {
        /// <summary>
        /// Full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added. E.g. "~/Cofoundry/MailTemplates/MyMailTemplate".
        /// </summary>
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(PasswordResetByAdminMailTemplate));

        /// <summary>
        /// String to use as the subject to the email.
        /// </summary>
        public string Subject { get; } = "Your password has been reset";

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
